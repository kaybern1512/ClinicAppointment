using ClinicBookingMVC.Models;
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

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApiKey"] ?? throw new InvalidOperationException("GeminiApiKey not configured.");
        }

        public async Task<ChatResponse> GetDoctorRecommendationAsync(string userMessage)
        {
            var response = new ChatResponse();

            try
            {
                var prompt = $@"You are a helpful medical assistant for ClinicAppointment website. Analyze the user's symptoms and provide:
1. Main symptoms identified
2. Recommended medical specialty  
3. Type of doctor to consult
4. Next steps (suggest booking appointment)

User symptoms: {userMessage}

Important: Respond in Vietnamese. Be concise, professional. Do not give medical diagnosis.";

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

                var url = $"{_baseUrl}/models/gemini-2.5-flash-exp:generateContent?key={_apiKey}";
                var httpResponse = await _httpClient.PostAsync(url, content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    var jsonDoc = JsonNode.Parse(responseContent);

                    if (jsonDoc != null && jsonDoc["candidates"]?.AsArray()?[0]?["content"]?["parts"]?.AsArray()?[0]?["text"] is JsonValue textNode)
                    {
                        response.Response = textNode.GetValue<string>()?.Trim() ?? "Không nhận được phản hồi từ AI.";
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
