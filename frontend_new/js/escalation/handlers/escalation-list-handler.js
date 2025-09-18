import { EscalationService } from '../services/escalation-service.js';
import { showError, clearMessages } from '../../utils/message-handler.js';

const escalationService = new EscalationService();

/**
 * Загрузка созданных пользователем эскалаций
 */
export const handleAuthoredEscalations = async (page = 1) => {
    try {
        clearMessages(document.getElementById('escalations-container'));
        const result = await escalationService.showAuthored();
        renderEscalations(result);
        updatePaginationInfo(result);
        updatePaginationButtons(result);

    } catch (error) {
        showError(document.getElementById('escalations-container'), error.message);
    }
};

/**
 * Загрузка назначенных пользователю эскалаций
 */
export const handleAssignedEscalations = async (page = 1) => {
    try {
        clearMessages(document.getElementById('escalations-container'));
        const result = await escalationService.showAssigned();
        renderEscalations(result);
        updatePaginationInfo(result);
        updatePaginationButtons(result);

    } catch (error) {
        showError(document.getElementById('escalations-container'), error.message);
    }
};

/**
 * Переход на следующую страницу
 */
export const nextPage = async () => {
    try {
        clearMessages(document.getElementById('escalations-container'));
        const result = await escalationService.nextPage();
        if (result) {
            renderEscalations(result);
            updatePaginationInfo(result);
            updatePaginationButtons(result);
        }
    } catch (error) {
        showError(document.getElementById('escalations-container'), error.message);
    }
};

/**
 * Переход на предыдущую страницу
 */
export const prevPage = async () => {
    try {
        clearMessages(document.getElementById('escalations-container'));
        const result = await escalationService.prevPage();
        if (result) {
            renderEscalations(result);
            updatePaginationInfo(result);
            updatePaginationButtons(result);
        }
    } catch (error) {
        showError(document.getElementById('escalations-container'), error.message);
    }
};

/**
 * Отрисовка списка эскалаций
 */
function renderEscalations(result) {
    const container = document.getElementById('escalations-container');
    if (!container) return;

    container.innerHTML = '';

    const escalations = result.escalations.$values || [];

    if (!escalations.length) {
        container.innerHTML = `
            <div class="alert alert-dismissible alert-light"">
                <i class="bi bi-info-circle"></i>
                ${result.type === 'authored'
            ? 'Вы не создали ни одной эскалации'
            : 'На вас не назначено эскалаций'}
            </div>
        `;
        return;
    }

    const list = document.createElement('div');
    list.className = 'list-group mb-4';

    escalations.forEach(esc => {
        const page = result.type === 'authored'
            ? 'escalation-details-author.html'
            : 'escalation-details-assigned.html';

        const featuredStar = esc.isFeatured
            ? '<span class="ms-2 p-1 bg-white"><i class="bi bi-star-fill text-warning"></i></span>'
            : '';

        const item = document.createElement('a');
        item.href = `${page}?id=${esc.id}`;
        item.className = 'list-group-item list-group-item-action';
        item.innerHTML = `
            <div class="d-flex justify-content-between align-items-start">
                <div class="flex-grow-1">
                    <h5 class="mb-1">${esc.name}</h5>
                    <p class="mb-1 text-muted">${esc.description || 'Без описания'}</p>
                    <small class="text-muted">Создано: ${new Date(esc.createdAt).toLocaleDateString()}</small>
                </div>
                <div class="d-flex align-items-center">
                    ${featuredStar}
                    <span class="badge ${escalationService.getStatusClass(esc.status)} ms-2">
                        ${escalationService.getStatusText(esc.status)}
                    </span>
                </div>
            </div>
        `;
        list.appendChild(item);
    });

    container.appendChild(list);
}

/**
 * Обновление информации о пагинации
 */
function updatePaginationInfo(result) {
    const searchInfo = document.getElementById('searchInfo');
    if (searchInfo) {
        searchInfo.textContent = result.paginationInfo;
    }
}

/**
 * Обновление состояния кнопок пагинации
 */
function updatePaginationButtons(result) {
    const prevBtn = document.getElementById('prevPage');
    const nextBtn = document.getElementById('nextPage');

    if (prevBtn) prevBtn.disabled = !result.hasPrev;
    if (nextBtn) nextBtn.disabled = !result.hasNext;
}