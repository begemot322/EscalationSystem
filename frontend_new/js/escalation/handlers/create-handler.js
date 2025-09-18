import { createEscalation } from '../../api/escalation-api.js';
import { UsersService } from '../services/users-service.js';
import { showError, showSuccess, clearMessages } from '../../utils/message-handler.js';

// Глобальный экземпляр сервиса
const usersService = new UsersService();

// Получаем контейнер для сообщений
const messageContainer = document.getElementById('message-container');

export async function initUsersList() {
    try {
        const result = await usersService.loadUsers();
        updateUsersSelect(result.users);
        updatePaginationInfo(result);
        updatePaginationButtons(result);

    } catch (error) {
        showError(messageContainer, error.message);
    }
}

export async function handleSearch() {
    const searchTerm = document.getElementById('userSearch').value.trim();

    try {
        const result = await usersService.searchUsers(searchTerm);
        updateUsersSelect(result.users);
        updatePaginationInfo(result);
        updatePaginationButtons(result);

    } catch (error) {
        showError(messageContainer, error.message);
    }
}

export async function nextPage() {
    try {
        const result = await usersService.nextPage();
        if (result) {
            updateUsersSelect(result.users);
            updatePaginationInfo(result);
            updatePaginationButtons(result);
        }
    } catch (error) {
        showError(messageContainer, error.message);
    }
}

export async function prevPage() {
    try {
        const result = await usersService.prevPage();
        if (result) {
            updateUsersSelect(result.users);
            updatePaginationInfo(result);
            updatePaginationButtons(result);
        }
    } catch (error) {
        showError(messageContainer, error.message);
    }
}

function updateUsersSelect(users) {
    const select = document.getElementById('responsibleUserIds');
    select.innerHTML = '';

    users.forEach((user, index) => {
        const option = document.createElement('option');
        option.value = user.id;
        option.textContent = usersService.formatUserOption(user, index);
        select.appendChild(option);
    });
}

function updatePaginationInfo(result) {
    const searchInfo = document.getElementById('searchInfo');
    if (searchInfo) {
        searchInfo.textContent = result.paginationInfo;
    }
}

function updatePaginationButtons(result) {
    const prevBtn = document.getElementById('prevPage');
    const nextBtn = document.getElementById('nextPage');

    if (prevBtn) prevBtn.disabled = !result.hasPrev;
    if (nextBtn) nextBtn.disabled = !result.hasNext;
}

export async function handleCreateEscalation() {
    const form = document.getElementById('escalation-form');

    const formData = {
        name: form.name.value.trim(),
        description: form.description.value.trim(),
        responsibleUserIds: Array.from(form.responsibleUserIds.selectedOptions)
            .map(option => parseInt(option.value))
    };

    // Очищаем предыдущие сообщения
    clearMessages(messageContainer);

    // Валидация
    if (!formData.name) {
        showError(messageContainer, 'Название эскалации обязательно');
        return;
    }
    if (!formData.description) {
        showError(messageContainer, 'Описание эскалации обязательно');
        return;
    }
    if (formData.responsibleUserIds.length === 0) {
        showError(messageContainer, 'Выберите хотя бы одного ответственного');
        return;
    }

    try {
        await createEscalation(formData);

        showSuccess(messageContainer, 'Эскалация успешно создана!', 'my-escalations.html');
        form.reset();

    } catch (error) {
        showError(messageContainer, error.message || 'Ошибка при создании эскалации');
    } finally {
        const submitButton = form.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.innerHTML = 'Создать эскалацию';
            submitButton.disabled = false;
        }
    }
}