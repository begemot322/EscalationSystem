import { EscalationDetailsService } from '../services/escalation-details-service.js';
import { ModalService } from '../services/modal-service.js';
import { showError, showSuccess } from '../../utils/message-handler.js';

let escalationService;
const messageContainer = document.getElementById('message-container');

export const loadAuthorEscalationDetails = async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const escalationId = urlParams.get('id');

    if (!escalationId) {
        showError('ID эскалации не указан');
        return;
    }

    escalationService = new EscalationDetailsService(escalationId);

    try {
        const [escalation, criteria,comments] = await Promise.all([
            escalationService.getEscalation(),
            escalationService.getCriteria(),
            escalationService.getComments()
        ]);

        renderEscalationDetails(escalation);
        renderCriteriaList(criteria);
        renderComments(comments);
        setupCriteriaHandlers();
        setupCommentHandler();


    } catch (error) {
        showError(messageContainer, 'Не удалось загрузить данные эскалации');
    }
};

function renderEscalationDetails(escalation) {
    const container = document.getElementById('escalation-details');
    if (!container) return;

    const featuredStar = escalation.isFeatured
        ? '<span class="ms-2 p-1 bg-black rounded"> <i class="bi bi-star-fill text-warning"></i></span>'
        : '';

    container.innerHTML = `
        <div class="card mb-4"> 
            <div class="card-header bg-black text-white d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center">
                    <h3 class="mb-0">Эскалация ${escalation.name}</h3>
                    ${featuredStar}
                </div>
                <button class="btn btn-sm btn-outline-light" id="edit-escalation-btn">
                    <i class="bi bi-pencil"></i> Редактировать
                </button>
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

    document.getElementById('edit-escalation-btn').addEventListener('click', () => {
        openEditModal(escalation);
    });
}

function renderCriteriaList(criteria) {
    const container = document.getElementById('criteria-list');
    if (!container) return;

    if (!criteria.length) {
        container.innerHTML = '<div class="alert alert-dismissible alert-light">' +
            '<span class="fw-bold text-decoration-underline">Добавьте первый критерий</span>' +
            '</div>';
        return;
    }

    const sortedCriteria = [...criteria].sort((a, b) => (a.order || 0) - (b.order || 0));

    container.innerHTML = sortedCriteria.map(criterion => `
        <div class="list-group-item ${criterion.isCompleted ? 'bg-light' : ''} mb-2 fs-5">
            <div class="d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center">
                    <span class="text-muted me-2" style="min-width: 20px;">${criterion.order || ''}.</span>
                    <i class="bi ${criterion.isCompleted ? 'bi-check-circle-fill text-success' : 'bi-circle text-secondary'} me-2"></i>
                    <span class="${criterion.isCompleted ? 'text-decoration-line-through' : ''}">
                        ${criterion.description}
                    </span>
                </div>
                <div class="btn-group">
                    <button class="btn btn-sm btn-outline-warning edit-criteria" data-id="${criterion.id}">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger delete-criteria" data-id="${criterion.id}">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </div>
        </div>
    `).join('');

    setupCriteriaEventListeners();
}

function setupCriteriaHandlers() {
    document.getElementById('show-add-criteria-form').addEventListener('click', showCriteriaForm);
    document.getElementById('add-criteria-btn').addEventListener('click', addCriterion);
    document.getElementById('cancel-add-criteria').addEventListener('click', hideCriteriaForm);
}

function showCriteriaForm() {
    document.getElementById('add-criteria-form').style.display = 'block';
    document.getElementById('show-add-criteria-form').style.display = 'none';
    document.getElementById('new-criteria-description').focus();
}

function hideCriteriaForm() {
    document.getElementById('add-criteria-form').style.display = 'none';
    document.getElementById('show-add-criteria-form').style.display = 'block';
    document.getElementById('new-criteria-description').value = '';
}

async function addCriterion() {
    const descriptionInput = document.getElementById('new-criteria-description');
    const description = descriptionInput.value.trim();

    if (!description) {
        showError('Введите описание критерия');
        return;
    }

    try {
        const addBtn = document.getElementById('add-criteria-btn');
        addBtn.disabled = true;
        addBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Добавление...';

        await escalationService.createCriterion(description);

        const criteria = await escalationService.getCriteria();
        renderCriteriaList(criteria);

        descriptionInput.value = '';
        hideCriteriaForm();
        showSuccess(messageContainer, 'Критерий успешно добавлен');

    } catch (error) {
        showError(messageContainer, 'Не удалось добавить критерий');
    } finally {
        const addBtn = document.getElementById('add-criteria-btn');
        if (addBtn) {
            addBtn.disabled = false;
            addBtn.textContent = 'Добавить';
        }
    }
}

function setupCriteriaEventListeners() {
    document.querySelectorAll('.edit-criteria').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const criteriaId = e.currentTarget.getAttribute('data-id');
            openEditCriteriaModal(criteriaId);
        });
    });

    document.querySelectorAll('.delete-criteria').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            const criteriaId = e.currentTarget.getAttribute('data-id');
            if (confirm('Вы уверены, что хотите удалить этот критерий?')) {
                await deleteCriterion(criteriaId);
            }
        });
    });
}

async function openEditCriteriaModal(criteriaId) {
    try {
        const criteria = await escalationService.getCriteria();
        const criterion = criteria.find(c => c.id.toString() === criteriaId);

        if (!criterion) {
            showError('Критерий не найден');
            return;
        }

        ModalService.createCriteriaModal(criterion, async (data) => {
            await escalationService.updateCriterion(criteriaId, {
                ...data,
                order: criterion.order || 0
            });

            const criteria = await escalationService.getCriteria();
            renderCriteriaList(criteria);
            showSuccess(messageContainer, 'Критерий успешно обновлен');
        });

    } catch (error) {
        showError(messageContainer, 'Ошибка при открытии редактора критерия');
    }
}

function openEditModal(escalation) {
    ModalService.createEscalationModal(escalation, async (data) => {
        await escalationService.updateEscalationData(data);

        const updatedEscalation = await escalationService.getEscalation();
        renderEscalationDetails(updatedEscalation);
        showSuccess(messageContainer, 'Эскалация успешно обновлена');
    });
}

async function deleteCriterion(criteriaId) {
    try {
        await escalationService.deleteCriterion(criteriaId);
        const criteria = await escalationService.getCriteria();
        renderCriteriaList(criteria);
        showSuccess(messageContainer, 'Критерий успешно удален');
    } catch (error) {
        showError(messageContainer, 'Не удалось удалить критерий');
    }
}

async function renderComments(comments) {
    const container = document.getElementById('comments-list');
    if (!container) return;

    if (!comments.length) {
        container.innerHTML = '<div class="alert alert-dismissible alert-light">' +
            '<span class="text-decoration-underline">Комментариев пока нет</span>' +
            '</div>';
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
    const addCommentBtn = document.getElementById('add-comment-btn');
    if (addCommentBtn) {
        addCommentBtn.addEventListener('click', addComment);
    }
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