$(document).ready(function() {
    // Toggle Active/Inactive AJAX
    $('.toggle-active-btn').on('click', function() {
        const $btn = $(this);
        const userId = $btn.data('user-id');
        const isCurrentlyActive = $btn.data('current-status');
        
        if (!confirm(isCurrentlyActive ? 'Xác nhận KHÓA tài khoản này?' : 'Xác nhận KÍCH HOẠT tài khoản này?')) {
            return;
        }

        $btn.prop('disabled', true).html('<i class="bi bi-hourglass-split"></i>');

        $.post('/AdminUsers/ToggleActive', { id: userId }, function(response) {
            if (response.success) {
                // Update button and badge
                $btn.data('current-status', response.isActive)
                    .removeClass('btn-outline-success btn-outline-danger')
                    .addClass(response.isActive ? 'btn-outline-success' : 'btn-outline-danger')
                    .html(`<i class="bi ${response.isActive ? 'bi-unlock' : 'bi-lock'}"></i>`)
                    .prop('disabled', false)
                    .attr('title', response.isActive ? 'Khóa tài khoản' : 'Kích hoạt tài khoản');

                // Update status badge if in table
                const $row = $btn.closest('tr');
                const $badge = $row.find('.badge');
                if ($badge.length) {
                    $badge.removeClass('bg-success bg-secondary')
                        .addClass(response.isActive ? 'bg-success' : 'bg-secondary')
                        .text(response.isActive ? 'Active' : 'Inactive');
                }

                // Success toast
                showToast(response.message, 'success');
            } else {
                showToast(response.message || 'Lỗi hệ thống', 'danger');
                $btn.prop('disabled', false).html(`<i class="bi ${isCurrentlyActive ? 'bi-unlock' : 'bi-lock'}"></i>`);
            }
        }).fail(function() {
            showToast('Lỗi kết nối. Thử lại.', 'danger');
            $btn.prop('disabled', false).html(`<i class="bi ${isCurrentlyActive ? 'bi-unlock' : 'bi-lock'}"></i>`);
        });
    });

    // Toast notification
    function showToast(message, type) {
        const toast = `
            <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0 position-fixed" 
                 style="top: 20px; right: 20px; z-index: 1055;" role="alert">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;
        $('body').append(toast);
        const bsToast = new bootstrap.Toast($('.toast')[0]);
        bsToast.show();
        bsToast._element.addEventListener('hidden.bs.toast', function () {
            $(this).remove();
        });
    }
});
