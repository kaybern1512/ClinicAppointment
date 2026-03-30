using ClinicBookingMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClinicBookingMVC.Services
{
    public interface IGeminiService
    {
        Task<ChatResponse> GetDoctorRecommendationAsync(string userMessage);
    }

    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://generativelanguage.googleapis.com/v1beta";

        private readonly ClinicBookingContext _context;
        
        public GeminiService(HttpClient httpClient, IConfiguration configuration, ClinicBookingContext context)
        {
            _httpClient = httpClient;
            _context = context;
            _apiKey = configuration["GeminiApiKey"] ?? throw new InvalidOperationException("GeminiApiKey not configured.");
        }

        public async Task<ChatResponse> GetDoctorRecommendationAsync(string userMessage)
        {
            var response = new ChatResponse();

            try
            {
                // Query real DB data for recommendations
                var specialties = await _context.Specialties
                    .Where(s => s.IsActive)
                    .Select(s => new { s.SpecialtyId, s.SpecialtyName, s.Description })
                    .ToListAsync();

                var doctors = await _context.Doctors
                    .Include(d => d.Specialty)
                    .Where(d => d.IsActive)
                    .Select(d => new {
                        d.DoctorId,
                        d.FullName,
                        SpecialtyName = d.Specialty.SpecialtyName,
                        SpecialtyId = d.SpecialtyId,
                        d.ExperienceYears,
                        d.ConsultationFee,
                        d.Description
                    })
                    .ToListAsync();

                // Group doctors by specialty for prompt
                var doctorsBySpecialty = doctors
                    .GroupBy(d => d.SpecialtyName)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var specialtyList = string.Join("\n", specialties.Select(s => $"• {s.SpecialtyName} (ID: {s.SpecialtyId})"));
                var doctorList = string.Join("\n", doctorsBySpecialty.Select(kvp => 
                    $"**{kvp.Key}:**\n" + string.Join("\n", kvp.Value.Select(d => $"  - ID:{d.DoctorId} {d.FullName} ({d.ExperienceYears} năm, {d.ConsultationFee:##,###}đ)"))
                ));

                var prompt = $@"Bạn là trợ lý y tế thông minh cho ClinicAppointment. Dựa trên triệu chứng, CHỌN BÁC SĨ THỰC TẾ từ danh sách sau:

**Chuyên khoa có sẵn:**
{specialtyList}

**Bác sĩ theo chuyên khoa:**
{doctorList}

Triệu chứng bệnh nhân: {userMessage}

✅ TRẢ LỜI CHỈ JSON thuần túy, KHÔNG text thêm nào khác!

{{
  ""specialtyId"": [ID số],
  ""specialtyName"": ""Tên chính xác"",
  ""doctorId"": [ID số], 
  ""doctorName"": ""Tên chính xác"",
  ""reason"": ""Giải thích ngắn gọn (tiếng Việt)"",
  ""response"": ""Tin nhắn thân thiện cho bệnh nhân (tiếng Việt)""
}}

⚠️ JSON THUẨN TÚY, KHÔNG text trước/sau. Không chẩn đoán y tế!";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topK = 40,
                        topP = 0.95,
                        maxOutputTokens = 1024,
                    },
                    safetySettings = new[]
                    {
                        new
                        {
                            category = "HARM_CATEGORY_HARASSMENT",
                            threshold = "BLOCK_MEDIUM_AND_ABOVE"
                        },
                        new
                        {
                            category = "HARM_CATEGORY_HATE_SPEECH",
                            threshold = "BLOCK_MEDIUM_AND_ABOVE"
                        },
                        new
                        {
                            category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                            threshold = "BLOCK_MEDIUM_AND_ABOVE"
                        },
                        new
                        {
                            category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                            threshold = "BLOCK_MEDIUM_AND_ABOVE"
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_baseUrl}/models/gemini-2.5-flash:generateContent?key={_apiKey}";
                var httpResponse = await _httpClient.PostAsync(url, content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    var jsonDoc = JsonNode.Parse(responseContent);

                    if (jsonDoc != null && jsonDoc["candidates"]?.AsArray()?[0]?["content"]?["parts"]?.AsArray()?[0]?["text"] is JsonValue textNode)
                    {
                        var aiText = textNode.GetValue<string>()?.Trim() ?? "";
                        
                        string parseText = aiText;

                        // Robust JSON extraction: find first complete JSON object in text
                        var jsonMatch = System.Text.RegularExpressions.Regex.Match(aiText, @"\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\}");
                        if (jsonMatch.Success)
                        {
                            parseText = jsonMatch.Value;
                        }

                        // Try parse JSON recommendation from AI response
                        try
                        {
                            using var doc = JsonDocument.Parse(parseText);
                            var root = doc.RootElement;
                            
                            response.RecommendedSpecialtyId = root.TryGetProperty("specialtyId", out var spId) ? spId.GetInt32() : null;
                            response.RecommendedSpecialtyName = root.TryGetProperty("specialtyName", out var spName) ? spName.GetString() ?? "" : "";
                            response.RecommendedDoctorId = root.TryGetProperty("doctorId", out var docId) ? docId.GetInt32() : null;
                            response.RecommendedDoctorName = root.TryGetProperty("doctorName", out var docName) ? docName.GetString() ?? "" : "";
                            response.RecommendationReason = root.TryGetProperty("reason", out var reason) ? reason.GetString() ?? "" : "";
                            response.Response = root.TryGetProperty("response", out var resp) ? resp.GetString() ?? parseText : parseText;
                            
                            // Populate available doctors for recommended specialty
                            if (response.RecommendedSpecialtyId.HasValue)
                            {
                                var recSpecialtyDoctors = doctors
                                    .Where(d => d.SpecialtyId == response.RecommendedSpecialtyId.Value)
                                    .Select(d => new DoctorRecommendation
                                    {
                                        DoctorId = d.DoctorId,
                                        DoctorName = d.FullName,
                                        SpecialtyName = d.SpecialtyName,
                                        ExperienceYears = d.ExperienceYears,
                                        ConsultationFee = d.ConsultationFee
                                    })
                                    .ToList();
                                response.AvailableDoctors = recSpecialtyDoctors;
                            }
                        }
                        catch (JsonException)
                        {
                            // Clean fallback - no raw text exposure
                            response.Response = "🤖 Tôi đã phân tích triệu chứng của bạn nhưng chưa thể đưa ra khuyến nghị cụ thể. Hãy mô tả thêm chi tiết hoặc thử triệu chứng khác nhé! 😊";

                        }
                    }
                    else
                    {
                        response.Response = "Phản hồi không đúng định dạng.";
                    }
                }
                else
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    response.IsError = true;
                    response.ErrorMessage = $"Lỗi API: {httpResponse.StatusCode} - {errorContent}";
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }

            return response;
        }
    }
}
