import { login, register } from '../api/auth-api.js';
import { showError, showSuccess, clearMessages } from '../utils/message-handler.js';

export const handleLogin = async () => {
    const form = document.getElementById('auth-form');
    const messageContainer = document.getElementById('message-container');
    const email = form.email.value;
    const password = form.password.value;

    clearMessages(messageContainer);

    try {
        await login(email, password);
        showSuccess(messageContainer, 'Вход выполнен успешно!');
        setTimeout(() => {
            window.location.href = 'index.html';
        }, 1500);
    } catch (error) {
        showError(messageContainer, error.message);
    }
};

export const handleRegister = async () => {
    const form = document.getElementById('register-form');
    const messageContainer = document.getElementById('message-container');

    const firstName = form.firstName.value;
    const lastName = form.lastName.value;
    const email = form.email.value;
    const phoneNumber = form.phoneNumber.value;
    const password = form.password.value;
    const role = form.role.value;

    clearMessages(messageContainer);

    try {
        await register({firstName,lastName, email, phoneNumber, password, role: parseInt(role)});
        showSuccess(messageContainer, 'Регистрация успешна!', 'login.html');
    } catch (error) {
        showError(messageContainer, error.message);
    }
};