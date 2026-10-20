function closeToast(id) {
    const toast = document.getElementById(id);
    if (toast) {
        toast.classList.add('hiding');
        setTimeout(() => {
            toast.remove();
        }, 300);
    }
}

// Auto-dismiss after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const toasts = document.querySelectorAll('.notification-toast');
    toasts.forEach(toast => {
        setTimeout(() => {
            closeToast(toast.id);
        }, 5000);
    });
});