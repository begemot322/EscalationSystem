export const showMessage = (container, type, message, redirectUrl = null, autoDismiss = true, dismissTime = 2000) => {
    if (!container) return;

    // Очищаем контейнер
    container.innerHTML = '';

    // Создаем сообщение
    const messageElement = document.createElement('div');
    messageElement.className = `alert alert-${type} alert-dismissible fade show`;
    messageElement.innerHTML = `
        <i class="bi ${type === 'success' ? 'bi-check-circle-fill' : 'bi-exclamation-triangle'}"></i> 
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    container.appendChild(messageElement);

    if (autoDismiss) {
        setTimeout(() => {
            messageElement.classList.remove('show');
            messageElement.classList.add('fade');

            setTimeout(() => {
                if (messageElement.parentNode === container) {
                    container.removeChild(messageElement);
                }
            }, 500);
        }, dismissTime);
    }

    // Редирект для успешных сообщений
    if (type === 'success' && redirectUrl) {
        setTimeout(() => {
            window.location.href = redirectUrl;
        }, 1500);
    }
};

export const showError = (container, message) => {
    showMessage(container, 'danger', message);
};

export const showSuccess = (container, message, redirectUrl = null) => {
    showMessage(container, 'success', message, redirectUrl);
};

export const clearMessages = (container) => {
    if (container) {
        container.innerHTML = '';
    }
};