// Инициализация стилей
export function initUiHelpers() {
    if (!document.getElementById('ui-helpers-styles')) {
        const style = document.createElement('style');
        style.id = 'ui-helpers-styles';
        style.textContent = `
            .fade-out { opacity: 0; transition: opacity 0.5s; }
            .auth-error { color: #dc3545; margin-top: 0.5rem; }
        `;
        document.head.appendChild(style);
    }
}

// Показать ошибку
export function showError(message, container = 'body', duration = 0) {
    const containerEl = typeof container === 'string'
        ? document.querySelector(container)
        : container;

    clearError(containerEl);

    const errorEl = document.createElement('div');
    errorEl.className = 'auth-error';
    errorEl.textContent = message;

    const target = containerEl.querySelector('button') || containerEl;
    target.after(errorEl);

    if (duration > 0) {
        setTimeout(() => errorEl.remove(), duration);
    }
}

// Показать успешное сообщение
export function showSuccess(message, container = 'body', duration = 3000) {
    const containerEl = typeof container === 'string'
        ? document.querySelector(container)
        : container;

    clearSuccess(containerEl);

    const successEl = document.createElement('div');
    successEl.className = 'alert alert-success mt-3';
    successEl.innerHTML = `
        <i class="bi bi-check-circle-fill"></i> ${message}
        ${duration > 0 ? '<div class="progress mt-2"><div class="progress-bar progress-bar-striped progress-bar-animated" style="width: 100%"></div></div>' : ''}
    `;

    containerEl.prepend(successEl);

    if (duration > 0) {
        setTimeout(() => {
            successEl.classList.add('fade-out');
            setTimeout(() => successEl.remove(), 500);
        }, duration);
    }
}

// Показать загрузку
export function showLoading(buttonOrContainer) {
    const target = typeof buttonOrContainer === 'string'
        ? document.querySelector(buttonOrContainer)
        : buttonOrContainer;

    if (target.tagName === 'BUTTON') {
        target.dataset.originalText = target.innerHTML;
        target.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Обработка...';
        target.disabled = true;
    } else {
        const loader = document.createElement('div');
        loader.className = 'text-center my-4';
        loader.innerHTML = `
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Загрузка...</span>
            </div>
        `;
        target.append(loader);
    }
}

// Скрыть загрузку
export function hideLoading(buttonOrContainer) {
    const target = typeof buttonOrContainer === 'string'
        ? document.querySelector(buttonOrContainer)
        : buttonOrContainer;

    if (target.tagName === 'BUTTON') {
        target.innerHTML = target.dataset.originalText || 'Submit';
        target.disabled = false;
    } else {
        const loader = target.querySelector('.spinner-border');
        if (loader) loader.remove();
    }
}

// Очистить ошибки
export function clearError(container = 'body') {
    const containerEl = typeof container === 'string'
        ? document.querySelector(container)
        : container;
    const error = containerEl.querySelector('.auth-error');
    if (error) error.remove();
}

// Очистить успешные сообщения
export function clearSuccess(container = 'body') {
    const containerEl = typeof container === 'string'
        ? document.querySelector(container)
        : container;
    const success = containerEl.querySelector('.alert-success');
    if (success) success.remove();
}