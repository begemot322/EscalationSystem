import { EscalationDetailsService } from '../services/escalation-details-service.js';
import { showError, showSuccess } from '../../utils/message-handler.js';

let escalationService;

const messageContainer = document.getElementById('message-container');


export const loadAssignedEscalationDetails = async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const escalationId = urlParams.get('id');

    if (!escalationId) {
        showError('ID эскалации не указан');
        return;
    }

    escalationService = new EscalationDetailsService(escalationId);

    try {
        const [escalation, criteria, comments] = await Promise.all([
            escalationService.getEscalation(),
            escalationService.getCriteria(),
            escalationService.getComments()
        ]);

        renderEscalationDetails(escalation);
        renderCriteriaList(criteria);
        renderComments(comments);
        setupCommentHandler();

    } catch (error) {
        showError(messageContainer,'Не удалось загрузить данные эскалации');
    }
};

function renderEscalationDetails(escalation) {
    const container = document.getElementById('escalation-details');
    if (!container) return;

    // Звездочка рядом с названием
    const featuredStar = escalation.isFeatured
        ? '<span class="ms-2 p-1 bg-black rounded"><i class="bi bi-star-fill text-warning"></i></span>'
        : '';

    container.innerHTML = `
        <div class="card mb-4"> 
            <div class="card-header bg-dark text-white d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center">
                    <h3 class="mb-0">Эскалация ${escalation.name}</h3>
                    ${featuredStar}
                </div>
            </div>
            <div class="card-body">
                <p class="card-text fs-5">Описание: ${escalation.description || 'Описание отсутствует'}</p>
                <div class="d-flex justify-content-between align-items-center">
                    <span class="badge ${escalationService.getStatusClass(escalation.status)}">
                        ${escalationService.getStatusText(escalation.status)}
                    </span>
                    <small class="text-muted">
                        Создано: ${new Date(escalation.createdAt).toLocaleDateString()}
                    </small>
                </div>
            </div>
        </div>
    `;
}

function renderCriteriaList(criteria) {
    const container = document.getElementById('criteria-list');
    if (!container) return;

    if (!criteria.length) {
        container.innerHTML = '<div class="alert alert-dismissible alert-light"><span class="text-decoration-underline">Критерии отсутствуют</span></div>';
        return;
    }

    const sortedCriteria = [...criteria].sort((a, b) => (a.order || 0) - (b.order || 0));

    container.innerHTML = sortedCriteria.map(criterion => `
        <div class="list-group-item ${criterion.isCompleted ? 'bg-light' : ''} fs-5 mb-2">
            <div class="d-flex align-items-center">
                <span class="text-muted me-2" style="min-width: 20px;">${criterion.order || ''}.</span>
                <i class="bi ${criterion.isCompleted ? 'bi-check-circle-fill text-success' : 'bi-circle text-secondary'} me-2"></i>
                <span class="${criterion.isCompleted ? 'text-decoration-line-through' : ''}">
                    ${criterion.description}
                </span>
            </div>
        </div>
    `).join('');
}

async function renderComments(comments) {
    const container = document.getElementById('comments-list');
    if (!container) return;

    if (!comments.length) {
        container.innerHTML = '<div class="alert alert-dismissible alert-light"><span class="text-decoration-underline">Комментариев пока нет</span></div>';
        return;
    }

    const commentsHtml = await Promise.all(comments.map(async comment => {
        const user = await escalationService.getUser(comment.userId);
        return `
            <div class="card mb-2">
                <div class="card-body p-3">
                    <div class="d-flex justify-content-between mb-2">
                        <strong>${user.firstName} ${user.lastName}</strong>
                        <small class="text-muted">${new Date(comment.createdAt).toLocaleDateString()}</small>
                    </div>
                    <p class="mb-0">${comment.text}</p>
                </div>
            </div>
        `;
    }));

    container.innerHTML = commentsHtml.join('');
}

function setupCommentHandler() {
    document.getElementById('add-comment-btn').addEventListener('click', addComment);
}

async function addComment() {
    const textarea = document.getElementById('new-comment-text');
    const text = textarea.value.trim();

    if (!text) {
        showError('Введите текст комментария');
        return;
    }

    try {
        await escalationService.createComment(text);
        textarea.value = '';
        showSuccess(messageContainer,'Комментарий добавлен');

        // Обновляем комментарии
        const comments = await escalationService.getComments();
        renderComments(comments);

    } catch (error) {
        showError(messageContainer,'Не удалось добавить комментарий');
    }
}