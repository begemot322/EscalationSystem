import { getEscalationById,updateEscalation  } from '../api/escalation-api.js';
import { getCriteriaByEscalationId, updateCriteria,createCriteria,deleteCriteria  } from '../api/criteria-api.js';
import { getCommentsByEscalationId, createComment } from '../api/comment-api.js';
import {getUserById} from "../api/user-api.js";

export const loadEscalationDetails = async () => {
    const urlParams = new URLSearchParams(window.location.search);
    const escalationId = urlParams.get('id');
    const viewMode = urlParams.get('view');

    console.log(`Escalation ID: ${escalationId}, View mode: ${viewMode}`); // Отладочный вывод

    const isCommentMode = viewMode === 'comment';
    const isEditMode = viewMode === 'edit';
    const isReadOnly = !isEditMode && !isCommentMode;

    try {
        console.log('Fetching escalation details...');
        const escalation = await getEscalationById(escalationId);
        console.log('Escalation data:', escalation);

        const criteriaResponse = await getCriteriaByEscalationId(escalationId);
        const criteria = criteriaResponse.$values || criteriaResponse;

        const comments = await getCommentsByEscalationId(escalationId);
        console.log('Comments data:', comments);

        renderEscalationDetails(escalation, isEditMode);
        renderCriteriaList(criteria, escalation.status, !isEditMode, escalationId);
        renderComments(comments, isCommentMode, escalationId);

        if (isEditMode) {
            console.log('Setting up criteria handlers');
            setupCriteriaFormHandlers(escalationId);
        }

        if (isCommentMode) {
            console.log('Setting up comment handler');
            setupCommentFormHandler(escalationId);
        }

    } catch (error) {
        console.error('Error loading escalation details:', error);
        showError(error.message);
    }
};
async function renderComments(comments, canComment, escalationId) {
    const container = document.getElementById('comments-list');
    if (!container) return;

    const commentsArray = comments?.$values || comments || [];

    if (canComment) {
        const addForm = document.getElementById('add-comment-form');
        if (addForm) addForm.style.display = 'block';
    }

    if (!commentsArray.length) {
        container.innerHTML = '<div class="alert alert-info">Комментариев пока нет</div>';
        return;
    }

    container.innerHTML = '';

    for (const comment of commentsArray) {
        try {
            const user = await getUserById(comment.userId);
            const commentElement = document.createElement('div');
            commentElement.className = 'card mb-2';
            commentElement.innerHTML = `
                <div class="card-body p-3">
                    <div class="d-flex justify-content-between mb-2">
                        <strong>${user.firstName} ${user.lastName}</strong>
                    </div>
                    <p class="mb-0">${comment.text}</p>
                </div>
            `;
            container.appendChild(commentElement);
        } catch (error) {
            const commentElement = document.createElement('div');
            commentElement.className = 'card mb-2';
            commentElement.innerHTML = `
                <div class="card-body p-3">
                    <div class="d-flex justify-content-between mb-2">
                        <strong>Аноним</strong>
                    </div>
                    <p class="mb-0">${comment.text}</p>
                </div>
            `;
            container.appendChild(commentElement);
        }
    }
}

function setupCommentFormHandler(escalationId) {
    const addBtn = document.getElementById('add-comment-btn');
    const textArea = document.getElementById('new-comment-text');

    if (!addBtn || !textArea) {
        return;
    }

    addBtn.addEventListener('click', async () => {
        const text = textArea.value.trim();
        if (!text) {
            showError('Введите текст комментария');
            return;
        }

        try {
            addBtn.disabled = true;
            addBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Отправка...';

            await createComment(escalationId, text); // Изменено здесь!

            const comments = await getCommentsByEscalationId(escalationId);
            await renderComments(comments, false, escalationId);

            // Очищаем поле ввода
            textArea.value = '';
            showSuccess('Комментарий успешно добавлен');
        } catch (error) {
            showError(error.message || 'Не удалось добавить комментарий');
        } finally {
            addBtn.disabled = false;
            addBtn.textContent = 'Отправить';
        }
    });

    textArea.addEventListener('keypress', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            addBtn.click();
        }
    });
}
function setupCriteriaFormHandlers(escalationId, isReadOnly) {
    const showFormBtn = document.getElementById('show-add-criteria-form');
    const addForm = document.getElementById('add-criteria-form');
    const addBtn = document.getElementById('add-criteria-btn');
    const cancelBtn = document.getElementById('cancel-add-criteria');
    const descriptionInput = document.getElementById('new-criteria-description');

    if (!showFormBtn || !addForm || !addBtn || !cancelBtn || !descriptionInput) {
        console.error('One or more form elements not found');
        return;
    }

    showFormBtn.addEventListener('click', () => {
        addForm.style.display = 'block';
        showFormBtn.style.display = 'none';
        descriptionInput.focus();
    });

    cancelBtn.addEventListener('click', () => {
        addForm.style.display = 'none';
        showFormBtn.style.display = 'block';
        descriptionInput.value = '';
    });

    addBtn.addEventListener('click', async () => {
        const description = descriptionInput.value.trim();
        if (!description) {
            showError('Введите описание критерия');
            return;
        }
        const criteriaResponse = await getCriteriaByEscalationId(escalationId);
        const criteria = criteriaResponse.$values || criteriaResponse;

        const escalation = await getEscalationById(escalationId);
        renderCriteriaList(criteria, escalation.status, isReadOnly, escalationId);

        try {
            addBtn.disabled = true;
            addBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Добавление...';

            await createCriteria(escalationId, { description });

            const criteriaResponse = await getCriteriaByEscalationId(escalationId);
            const criteria = criteriaResponse.$values || criteriaResponse;
            renderCriteriaList(criteria, escalation.status, isReadOnly, escalationId);

            descriptionInput.value = '';
            addForm.style.display = 'none';
            showFormBtn.style.display = 'block';
            showSuccess('Критерий успешно добавлен');

        } catch (error) {
            showError(error.message);
        } finally {
            addBtn.disabled = false;
            addBtn.textContent = 'Добавить';
        }
    });

    descriptionInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            addBtn.click();
        }
    });
}

function renderEscalationDetails(escalation, canEdit = false) {
    const container = document.getElementById('escalation-details');
    if (!container) {
        console.error('Element with ID "escalation-details" not found');
        return;
    }

    const escalationData = escalation.$values ? escalation.$values[0] : escalation;

    container.innerHTML = `
        <div class="card mb-4">
            <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                <h3>${escalationData.name}</h3>
                ${canEdit ? `
                <button class="btn btn-sm btn-outline-light" id="edit-escalation-btn">
                    <i class="bi bi-pencil"></i> Редактировать
                </button>
                ` : ''}
            </div>
            <div class="card-body">
                <p class="card-text">${escalationData.description || 'Описание отсутствует'}</p>
                <div class="d-flex justify-content-between align-items-center">
                    <span class="badge ${getStatusClass(escalationData.status)}">
                        ${getStatusText(escalationData.status)}
                    </span>
                    <small class="text-muted">
                        Создано: ${new Date(escalationData.createdAt).toLocaleDateString()}
                    </small>
                </div>
            </div>
        </div>
    `;

    if (canEdit) {
        document.getElementById('edit-escalation-btn')?.addEventListener('click', () => {
            openEditEscalationModal(escalationData);
        });
    }
}

// Добавим функцию для  окна редактирования
function openEditEscalationModal(escalation) {
    const modalContainer = document.createElement('div');
    modalContainer.innerHTML = `
        <div class="modal fade" id="editEscalationModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Редактировать эскалацию</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <form id="edit-escalation-form">
                            <div class="mb-3">
                                <label class="form-label">Название</label>
                                <input type="text" class="form-control" id="edit-escalation-name" 
                                       value="${escalation.name || ''}" required>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Описание</label>
                                <textarea class="form-control" id="edit-escalation-description" 
                                          rows="3">${escalation.description || ''}</textarea>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Статус</label>
                                <select class="form-select" id="edit-escalation-status">
                                    <option value="0" ${escalation.status === 0 ? 'selected' : ''}>Новая</option>
                                    <option value="1" ${escalation.status === 1 ? 'selected' : ''}>В работе</option>
                                    <option value="2" ${escalation.status === 2 ? 'selected' : ''}>На проверке</option>
                                    <option value="3" ${escalation.status === 3 ? 'selected' : ''}>Завершена</option>
                                    <option value="4" ${escalation.status === 4 ? 'selected' : ''}>Отклонена</option>
                                </select>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
                        <button type="button" class="btn btn-primary" id="save-escalation-btn">
                            Сохранить
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;

    document.body.appendChild(modalContainer);

    const modal = new bootstrap.Modal(document.getElementById('editEscalationModal'));
    modal.show();

    document.getElementById('save-escalation-btn').addEventListener('click', async () => {
        try {
            const name = document.getElementById('edit-escalation-name').value.trim();
            const description = document.getElementById('edit-escalation-description').value.trim();
            const status = parseInt(document.getElementById('edit-escalation-status').value);

            if (!name) {
                showError('Введите название эскалации');
                return;
            }

            await updateEscalation(escalation.id, {
                name: name,
                description: description,
                status: status
            });

            modal.hide();
            showSuccess('Эскалация успешно обновлена');
            loadEscalationDetails();
        } catch (error) {
            showError(error.message);
        }
    });
}
function renderCriteriaList(criteriaList, escalationStatus, isReadOnly = false, escalationId = null) {
    const container = document.getElementById('criteria-list');
    if (!container) {
        console.error('Container for criteria list not found');
        return;
    }

    const urlParams = new URLSearchParams(window.location.search);
    const criteria = criteriaList?.$values || criteriaList || [];

    const addForm = document.getElementById('add-criteria-form');
    const showButton = document.getElementById('show-add-criteria-form');

    if (isReadOnly) {
        if (addForm) addForm.style.display = 'none';
        if (showButton) showButton.style.display = 'none';
    } else {
        if (showButton) showButton.style.display = 'block';
    }

    if (!criteria.length) {
        container.innerHTML = isReadOnly
            ? '<div class="alert alert-info">Критерии отсутствуют</div>'
            : '<div class="alert alert-info">Нет критериев. Добавьте первый критерий.</div>';
        return;
    }

    const sortedItems = [...criteria].sort((a, b) => (a.order || 0) - (b.order || 0));

    container.innerHTML = `
        <div class="list-group" id="criteria-items-container"></div>
    `;

    const itemsContainer = document.getElementById('criteria-items-container');
    if (!itemsContainer) return;

    sortedItems.forEach(item => {
        const criteria = item.$values ? item.$values[0] : item;
        if (!criteria) return;

        const element = document.createElement('div');
        element.className = `list-group-item ${criteria.isCompleted ? 'bg-light' : ''}`;

        element.innerHTML = `
            <div class="d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center">
                    <div class="me-3">
                        <i class="bi ${criteria.isCompleted ? 'bi-check-circle-fill text-success' : 'bi-circle text-secondary'}" 
                           style="font-size: 1.2rem;"></i>
                    </div>
                    <label class="form-check-label">
                        <span class="${criteria.isCompleted ? 'text-decoration-line-through' : ''}">
                            ${criteria.order || ''}. ${criteria.description || ''}
                        </span>
                    </label>
                </div>
                <div>
                    ${criteria.isCompleted ?
            '<span class="badge bg-success">Выполнен</span>' :
            '<span class="badge bg-warning text-dark">В работе</span>'}
                    ${!isReadOnly ? `
                    <div class="btn-group ms-2">
                        <button class="btn btn-sm btn-outline-primary edit-criteria-btn" data-id="${criteria.id}">
                            <i class="bi bi-pencil"></i>
                        </button>
                        <button class="btn btn-sm btn-outline-danger delete-criteria-btn" data-id="${criteria.id}">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                    ` : ''}
                </div>
            </div>
        `;

        itemsContainer.appendChild(element);
    });

    if (!isReadOnly) {
        document.querySelectorAll('.edit-criteria-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                const criteriaId = e.currentTarget.getAttribute('data-id');
                const selectedCriteria = criteria.find(c => c.id.toString() === criteriaId);
                if (selectedCriteria) {
                    openEditCriteriaModal(escalationId, selectedCriteria, isReadOnly);
                }
            });
        });

        document.querySelectorAll('.delete-criteria-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const criteriaId = e.currentTarget.getAttribute('data-id');
                handleDeleteCriteria(escalationId, criteriaId, isReadOnly);
            });
        });
    }
}

async function handleDeleteCriteria(escalationId, criteriaId, isReadOnly) {
    if (!confirm('Вы уверены, что хотите удалить этот критерий?')) {
        return;
    }

    try {
        await deleteCriteria(escalationId, criteriaId);
        showSuccess('Критерий успешно удален');

        // Обновляем список
        const escalation = await getEscalationById(escalationId);
        const criteriaResponse = await getCriteriaByEscalationId(escalationId);
        const criteria = criteriaResponse.$values || criteriaResponse;
        renderCriteriaList(criteria, escalation.status, isReadOnly, escalationId);

    } catch (error) {
        showError(error.message);
    }
}

async function handleCriteriaToggle(escalationId, criteriaId, isCompleted) {
    try {
        await updateCriteria(escalationId, criteriaId, { isCompleted });
        showSuccess('Статус критерия обновлен');
    } catch (error) {
        showError(error.message);
        // Возвращаем чекбокс в предыдущее состояние
        const checkbox = document.querySelector(`#criteria-${criteriaId}`);
        if (checkbox) {
            checkbox.checked = !isCompleted;
        }
    }
}

function openEditCriteriaModal(escalationId, criteria, isReadOnly) {
    console.log('Attempting to open modal with:', { escalationId, criteria, isReadOnly });

    const existingModal = document.getElementById('editCriteriaModal');
    if (existingModal) {
        console.log('Removing existing modal');
        existingModal.remove();
    }

    // Создаем новое модальное окно
    const modalContainer = document.createElement('div');
    modalContainer.innerHTML = `
        <div class="modal fade" id="editCriteriaModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Редактировать критерий</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <form id="edit-criteria-form">
                            <div class="mb-3">
                                <label class="form-label">Описание</label>
                                <textarea class="form-control" id="edit-criteria-description" rows="3" required>${criteria.description || ''}</textarea>
                            </div>
                            <div class="form-check mb-3">
                                <input class="form-check-input" type="checkbox" id="edit-criteria-completed" ${criteria.isCompleted ? 'checked' : ''}>
                                <label class="form-check-label" for="edit-criteria-completed">
                                    Выполнено
                                </label>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
                        <button type="button" class="btn btn-primary" id="save-criteria-btn">Сохранить</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    document.body.appendChild(modalContainer);
    console.log('Modal HTML added to DOM');

    const modalElement = document.getElementById('editCriteriaModal');
    if (!modalElement) {
        console.error('Modal element not found after creation!');
        return;
    }

    try {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
        console.log('Modal shown successfully');
    } catch (e) {
        console.error('Error initializing modal:', e);
        return;
    }

    // Обработчик сохранения
    document.getElementById('save-criteria-btn').addEventListener('click', async () => {
        console.log('Save button clicked');
        const description = document.getElementById('edit-criteria-description').value.trim();
        const isCompleted = document.getElementById('edit-criteria-completed').checked;

        if (!description) {
            showError('Введите описание критерия');
            return;
        }

        try {
            const dto = {
                description: description,
                isCompleted: isCompleted,
                order: criteria.order || 0
            };

            console.log('Updating criteria with:', dto);
            await updateCriteria(escalationId, criteria.id, dto);

            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) {
                modal.hide();
            }

            showSuccess('Критерий успешно обновлен');

            // Обновляем список
            const escalation = await getEscalationById(escalationId);
            const criteriaResponse = await getCriteriaByEscalationId(escalationId);
            const updatedCriteria = criteriaResponse.$values || criteriaResponse;
            renderCriteriaList(updatedCriteria, escalation.status, isReadOnly, escalationId);
        } catch (error) {
            console.error('Error updating criteria:', error);
            showError(error.message);
        }
    });

    modalElement.addEventListener('hidden.bs.modal', () => {
        console.log('Modal hidden, removing from DOM');
        modalContainer.remove();
    });
}

function showError(message) {
    const container = document.getElementById('escalation-container') || document.body;

    const oldAlerts = container.querySelectorAll('.alert.alert-danger');
    oldAlerts.forEach(alert => alert.remove());

    const errorElement = document.createElement('div');
    errorElement.className = 'alert alert-danger mt-3';
    errorElement.textContent = message;
    container.prepend(errorElement);

    setTimeout(() => errorElement.remove(), 5000);
}


function showSuccess(message) {
    const container = document.getElementById('escalation-container') || document.body;

    const oldAlerts = container.querySelectorAll('.alert.alert-success');
    oldAlerts.forEach(alert => alert.remove());

    const successElement = document.createElement('div');
    successElement.className = 'alert alert-success mt-3';
    successElement.textContent = message;
    container.prepend(successElement);

    setTimeout(() => successElement.remove(), 3000);
}

function getStatusClass(status) {
    if (typeof status === 'number') {
        const classes = {
            0: 'bg-secondary',
            1: 'bg-warning text-dark',
            2: 'bg-info text-white',
            3: 'bg-success',
            4: 'bg-danger',
        };
        return classes[status] || 'bg-secondary';
    }
}

function getStatusText(status) {
    if (typeof status === 'number') {
        const statusMap = {
            0: 'Новая',
            1: 'В работе',
            2: 'На проверке',
            3: 'Завершена',
            4: 'Отклонена'
        };
        return statusMap[status] || 'Неизвестный статус';
    }
}
