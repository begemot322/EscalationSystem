import { getFeaturedEscalations } from '../../api/escalation-api.js';

const getStatusText = (status) => {
    const statusMap = {
        0: 'Новая',
        1: 'В работе',
        2: 'На проверке',
        3: 'Завершена',
        4: 'Отклонена'
    };
    return statusMap[status] || `Статус ${status}`;
};

const getStatusClass = (status) => {
    const classMap = {
        0: 'bg-light text-dark',
        1: 'bg-primary text-white',
        2: 'bg-warning text-dark',
        3: 'bg-success text-white',
        4: 'bg-danger text-white'
    };
    return classMap[status] || 'bg-secondary text-white';
};

let isShowing = false;

export const renderFeaturedEscalations = async () => {
    if (isShowing) {
        hideEscalations();
        return;
    }

    const container = document.getElementById('featured-escalations');
    const escalationsContainer = document.getElementById('escalations-container');
    const showCasesBtn = document.getElementById('showCasesBtn');

    container.innerHTML = `
        <div class="col-12 text-center py-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Загрузка...</span>
            </div>
            <div class="mt-3 text-muted">Загрузка лучших решений...</div>
        </div>
    `;

    escalationsContainer.classList.remove('d-none');
    escalationsContainer.classList.add('animate__animated', 'animate__fadeIn');
    isShowing = true;

    try {
        const response = await getFeaturedEscalations();
        const escalations = response.$values || response;

        if (!escalations || escalations.length === 0) {
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <i class="bi bi-stars display-4 text-muted opacity-50"></i>
                    <h5 class="mt-4 text-muted">Пока нет избранных эскалаций</h5>
                    <p class="text-muted">Наши эксперты работают над новыми решениями</p>
                </div>
            `;
            return;
        }

        const sortedEscalations = escalations
            .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
            .slice(0, 3);

        container.innerHTML = sortedEscalations.map(escalation => `
            <div class="col-md-4 mb-4">
                <div class="card h-100 border-0 shadow-sm hover-shadow transition-all">
                    <div class="card-body text-center p-4">
                        <span class="badge ${getStatusClass(escalation.status)} mb-3 px-3 py-2 rounded-pill">
                            ${getStatusText(escalation.status)}
                        </span>
                        <h5 class="card-title fw-bold text-dark mb-3">${escalation.name}</h5>
                        <p class="card-text text-muted line-clamp-3">${escalation.description || 'Описание отсутствует'}</p>
                        
                        <div class="mt-4 pt-3 border-top border-light">
                            <small class="text-muted">
                                <i class="bi bi-calendar me-1"></i>
                                Создано: ${new Date(escalation.createdAt).toLocaleDateString('ru-RU')}
                            </small>
                        </div>
                        
                        ${escalation.isFeatured ?
            `<div class="mt-3">
                                <span class="badge bg-gradient-warning text-dark px-3 py-2 rounded-pill">
                                    <i class="bi bi-star-fill me-1"></i>Избранное
                                </span>
                            </div>` :
            ''}
                    </div>
                    <div class="card-footer bg-transparent border-0 pt-0 px-4 pb-4">
                        <small class="text-muted d-block text-center">
                            Номер: ${escalation.id}
                        </small>
                        <small class="text-muted d-block text-center mt-1">
                            <i class="bi bi-arrow-repeat me-1"></i>
                            Обновлено: ${new Date(escalation.updatedAt).toLocaleDateString('ru-RU')}
                        </small>
                    </div>
                </div>
            </div>
        `).join('');

        showCasesBtn.innerHTML = `
            <div class="text-white d-flex align-items-center">
                Скрыть кейсы <i class="bi bi-chevron-up ms-2"></i>
            </div>
        `;

    } catch (error) {
        container.innerHTML = `
            <div class="col-12 text-center py-4">
                <i class="bi bi-exclamation-triangle display-4 text-warning mb-3"></i>
                <h5 class="text-warning mb-3">Не удалось загрузить избранные эскалации</h5>
                <button class="btn btn-primary mt-2 px-4 py-2 rounded-pill" 
                        onclick="window.featuredEscalationsHandler.renderFeaturedEscalations()">
                    <i class="bi bi-arrow-clockwise me-2"></i>Попробовать снова
                </button>
            </div>
        `;
    }
};

export const hideEscalations = () => {
    const escalationsContainer = document.getElementById('escalations-container');
    const showCasesBtn = document.getElementById('showCasesBtn');

    escalationsContainer.classList.add('d-none');
    isShowing = false;

    showCasesBtn.innerHTML = `
        <div class="text-white d-flex align-items-center">
            Посмотреть кейсы <i class="bi bi-chevron-down ms-2"></i>
        </div>
    `;
};

export const initFeaturedEscalations = () => {
    const showCasesBtn = document.getElementById('showCasesBtn');
    if (showCasesBtn) {
        showCasesBtn.addEventListener('click', renderFeaturedEscalations);
    }
};

window.featuredEscalationsHandler = {
    renderFeaturedEscalations,
    hideEscalations,
    initFeaturedEscalations
};