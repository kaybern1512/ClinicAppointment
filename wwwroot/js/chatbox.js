// Chatbox JS - Professional recommendation UI (no raw JSON)
(function () {
    'use strict';

    let chatHistory = [];
    let isLoading = false;

    const CHATBOX_SELECTOR = '.chatbox-container';
    const FAB_SELECTOR = '.chatbox-fab';

    // Initialize
    function init() {
        createChatbox();
        bindEvents();
    }

    function createChatbox() {
        const fab = document.createElement('button');
        fab.className = 'chatbox-fab';
        fab.innerHTML = '💬';
        fab. title = 'Trợ giúp tư vấn bác sĩ';
        fab.onclick = toggleChatbox;
        document.body.appendChild(fab);

        const chatbox = document.createElement('div');
        chatbox.className = 'chatbox-container';
        chatbox.innerHTML = `
            <div class="chatbox-header" onclick="window.chatboxApp.toggleChatbox()">
                <button class="chatbox-toggle-btn" title="Đóng">×</button>
                <div class="chatbox-title">🤖 Trợ giúp tư vấn bác sĩ</div>
            </div>
            <div class="chatbox-body" id="chatMessages"></div>
            <div class="chatbox-input-container">
                <input type="text" class="chatbox-input" id="chatInput" placeholder="Mô tả triệu chứng của bạn..." maxlength="1000">
                <button class="chatbox-send-btn" id="chatSendBtn" disabled title="Gửi">➤</button>
            </div>
        `;
        document.body.appendChild(chatbox);
        window.chatboxApp = { 
            toggleChatbox: toggleChatbox,
            bookDoctor: function(specialtyId, doctorId, doctorName) {
                const params = new URLSearchParams({
                    specialtyId: specialtyId,
                    doctorId: doctorId
                });
                window.open(`/Appointments/Create?${params}`, '_blank');
                addMessage(`Đã mở form đặt lịch với ${doctorName}. Bạn có thể đóng tab chatbox.`, 'bot');
            }
        };
    }

    function bindEvents() {
        const input = document.getElementById('chatInput');
        const sendBtn = document.getElementById('chatSendBtn');
        const messages = document.getElementById('chatMessages');

        input.addEventListener('input', function() {
            toggleSendButton(this.value.trim());
        });

        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && !e.shiftKey && !isLoading) {
                e.preventDefault();
                sendMessage();
            }
        });

        sendBtn.addEventListener('click', sendMessage);

        // Auto scroll
        messages.addEventListener('scroll', function() {
            // Lazy load if needed
        });
    }

    function toggleSendButton(hasText) {
        const btn = document.getElementById('chatSendBtn');
        btn.disabled = !hasText || isLoading;
    }

    window.toggleChatbox = function() {
        const chatbox = document.querySelector(CHATBOX_SELECTOR);
        chatbox.classList.toggle('open');
    };

    // NEW: Professional recommendation rendering (replaces raw JSON)
    function addRecommendationMessage(data) {
        const specialtyId = data.recommendedSpecialtyId;
        const specialtyName = data.recommendedSpecialtyName || '';
        const doctorId = data.recommendedDoctorId;
        const doctorName = data.recommendedDoctorName || '';
        const reason = data.recommendationReason || 'Phù hợp với triệu chứng của bạn';
        const response = data.response || '';

        // Doctor list HTML
        let doctorListHtml = '';
        if (data.availableDoctors && data.availableDoctors.length > 0) {
            doctorListHtml = data.availableDoctors.map(doc => 
                `<li>
                    <a href="/Appointments/Create?specialtyId=${specialtyId}&doctorId=${doc.doctorId}" 
                       target="_blank" class="book-btn" style="padding: 8px 16px; font-size: 14px;">
                        ${doc.doctorName} • ${doc.experienceYears} năm • ${doc.consultationFee.toLocaleString('vi-VN')}đ
                    </a>
                </li>`
            ).join('');
        }

        const html = `
            <div class="message-bubble recommendation">
                <h4 style="margin: 0 0 10px; color: white;">📋 Khuyến nghị: <strong>${doctorName}</strong> (${specialtyName})</h4>
                ${response ? `<p style="margin: 0 0 15px; font-size: 15px;">${response.replace(/\n/g, '<br>')}</p>` : ''}
                <div class="recommendation-reason">
                    <strong>💭 Lý do:</strong> ${reason}
                </div>
                <div class="recommendation-actions">
                    <a href="/Appointments/Create?specialtyId=${specialtyId}&doctorId=${doctorId}" 
                       target="_blank" class="book-btn" style="margin-right: 10px;">
                        📅 Đặt lịch ngay
                    </a>
                    ${doctorListHtml ? `
                        <button onclick="showOtherDoctors(${specialtyId}, ${JSON.stringify(data.availableDoctors)})" 
                                class="other-doctors-btn">
                            👥 Bác sĩ khác
                        </button>
                    ` : ''}
                </div>
                ${doctorListHtml ? `
                    <details style="margin-top: 15px;">
                        <summary style="cursor: pointer; color: rgba(255,255,255,0.8); font-weight: 500;">👥 Xem tất cả bác sĩ ${specialtyName}</summary>
                        <div class="doctor-list">
                            <ul>${doctorListHtml}</ul>
                        </div>
                    </details>
                ` : ''}
            </div>
        `;

        addMessage(html, 'bot');
    }

    // Helper for other doctors (legacy + improved)
    window.showOtherDoctors = function(specialtyId, doctors) {
        let doctorList = doctors.map(d => 
            `<li style="margin: 5px 0;"><a href="/Appointments/Create?specialtyId=${specialtyId}&doctorId=${d.doctorId}" target="_blank" style="color: #007bff; text-decoration: none;">
                ${d.doctorName} • ${d.experienceYears} năm kinh nghiệm • ${d.consultationFee.toLocaleString('vi-VN')}đ
            </a></li>`
        ).join('');
        addMessage(`
            <div class="doctor-list" style="background: #f8f9fa; padding: 15px; border-radius: 10px;">
                <h4 style="margin: 0 0 10px 0; color: #495057;">👥 Các bác sĩ cùng chuyên khoa:</h4>
                <ul style="margin: 0; padding-left: 20px;">${doctorList}</ul>
            </div>
        `, 'bot');
    };

    async function sendMessage() {
        const input = document.getElementById('chatInput');
        const message = input.value.trim();

        if (!message || isLoading) return;

        addMessage(message, 'user');
        input.value = '';
        toggleSendButton(false);
        isLoading = true;

        addLoadingMessage();

        try {
            const response = await fetch('/Chat/Ask', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ message: message })
            });

            removeLoadingMessage();

            if (response.ok) {
                const data = await response.json();
                if (data.isError) {
                    addMessage(data.errorMessage || 'Có lỗi xảy ra. Vui lòng thử lại.', 'bot');
                } else if (data.recommendedDoctorId && data.recommendedSpecialtyId) {
                    addRecommendationMessage(data);
                } else {
                    addMessage(data.response || 'Không nhận được khuyến nghị cụ thể.', 'bot');
                }
            } else {
                addMessage('Không thể kết nối tới AI. Vui lòng thử lại.', 'bot');
            }
        } catch (error) {
            removeLoadingMessage();
            addMessage('Lỗi kết nối. Vui lòng thử lại sau.', 'bot');
            console.error('Chat error:', error);
        } finally {
            isLoading = false;
            toggleSendButton(true);
            scrollToBottom();
        }
    }

    function addMessage(text, sender) {
        const messages = document.getElementById('chatMessages');
        const time = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
        const div = document.createElement('div');
        div.className = `message ${sender}`;
        div.innerHTML = `
            <div class="message-bubble">${text.replace(/\\n/g, '<br>').replace(/&/g, '&amp;').replace(/</g, '<').replace(/>/g, '>')}</div>
            <div class="message-time">${time}</div>
        `;
        messages.appendChild(div);
        scrollToBottom();
    }

    function addLoadingMessage() {
        const messages = document.getElementById('chatMessages');
        const div = document.createElement('div');
        div.id = 'loadingMessage';
        div.className = 'message bot loading';
        div.innerHTML = `
            <div class="message-bubble">
                <div class="loading">
                    Đang tư vấn...
                    <div class="loading-dots">
                        <span></span><span></span><span></span>
                    </div>
                </div>
            </div>
        `;
        messages.appendChild(div);
        scrollToBottom();
    }

    function removeLoadingMessage() {
        const loading = document.getElementById('loadingMessage');
        if (loading) loading.remove();
    }

    function scrollToBottom() {
        const messages = document.getElementById('chatMessages');
        messages.scrollTop = messages.scrollHeight;
    }

    // Welcome message
    setTimeout(() => {
        if (document.querySelector(CHATBOX_SELECTOR)) {
            addMessage('Chào bạn! Tôi có thể giúp bạn tìm bác sĩ phù hợp dựa trên triệu chứng. Hãy mô tả tình trạng của bạn nhé! 😊', 'bot');
        }
    }, 1000);

    // Initialize when DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

