import { createEscalation } from '../api/escalation-api.js';
import { getUsers } from '../api/user-api.js';

export async function loadUsers() {
    try {
        const usersResponse = await getUsers({}, { pageSize: 100 });

        const select = document.getElementById('responsibleUserIds');
        usersResponse.items.forEach(user => {
            const option = document.createElement('option');
            option.value = user.id;

            const roleName = getRoleName(user.role);
            option.textContent = `${user.firstName} ${user.lastName} (${roleName})`;
            select.appendChild(option);
        });
    } catch (error) {
        showError('Не удалось загрузить список пользователей');
    }
}

export async function handleCreateEscalation() {
    const form = document.getElementById('escalation-form');
    const formData = {
        name: form.name.value,
        description: form.description.value,
        responsibleUserIds: Array.from(form.responsibleUserIds.selectedOptions)
            .map(option => parseInt(option.value))
    };

    clearNotifications();
    clearErrors();

    try {
        const submitButton = form.querySelector('button[type="submit"]');
        const originalButtonText = submitButton.innerHTML;
        submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Создание...';
        submitButton.disabled = true;

        const result = await createEscalation(formData);

        showSuccess('Эскалация успешно создана!');

        submitButton.innerHTML = originalButtonText;
        submitButton.disabled = false;

        form.reset();

        setTimeout(() => {
            window.location.href = 'index.html';
        }, 1500);

    } catch (error) {
        const submitButton = form.querySelector('button[type="submit"]');
        submitButton.innerHTML = 'Создать';
        submitButton.disabled = false;

        showError(error.message || "Ошибка при создании эскалации");
    }
}

function showError(message) {
    const errorElement = document.createElement('div');
    errorElement.className = 'alert alert-danger mt-3';
    errorElement.textContent = message;
    document.getElementById('escalation-form').appendChild(errorElement);
}

function showSuccess(message) {
    const successElement = document.createElement('div');
    successElement.className = 'alert alert-success mt-3';
    successElement.innerHTML = `
        <i class="bi bi-check-circle-fill"></i> ${message}
        <div class="progress mt-2">
            <div class="progress-bar progress-bar-striped progress-bar-animated" style="width: 100%"></div>
        </div>
    `;
    document.getElementById('escalation-form').appendChild(successElement);
}

function clearNotifications() {
    const oldSuccess = document.querySelector('.alert-success');
    if (oldSuccess) oldSuccess.remove();
}

function clearErrors() {
    const oldError = document.querySelector('.alert-danger');
    if (oldError) oldError.remove();
}

function getRoleName(role) {
    const roles = {
        0: 'Junior',
        1: 'Middle',
        2: 'Senior'
    };
    return roles[role] || 'Unknown';
}
