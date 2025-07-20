import { login,register } from '../api/auth-api.js';

export const handleLogin = async () => {
    const form = document.getElementById('auth-form');
    const email = form.email.value;
    const password = form.password.value;

    // Удаляем старые ошибки
    const oldError = form.querySelector('.auth-error');
    if (oldError) oldError.remove();

    try {
        await login(email, password);
        window.location.href = 'index.html';
    } catch (error) {
        const errorElement = document.createElement('div');
        errorElement.className = 'auth-error text-danger mt-2';
        errorElement.textContent = error.message;
        form.querySelector('button').after(errorElement);
    }
};

export const handleRegister = async () => {
    const form = document.getElementById('register-form');
    const firstName = form.firstName.value;
    const lastName = form.lastName.value;
    const email = form.email.value;
    const phoneNumber = form.phoneNumber.value;
    const password = form.password.value;
    const role = form.role.value;

    // Remove old errors
    const oldError = form.querySelector('.auth-error');
    if (oldError) oldError.remove();

    try {
        await register({
            firstName,
            lastName,
            email,
            phoneNumber,
            password,
            role: parseInt(role)
        });
        window.location.href = 'login.html';
    } catch (error) {
        const errorElement = document.createElement('div');
        errorElement.className = 'auth-error text-danger mt-2';
        errorElement.textContent = error.message;
        form.querySelector('button').after(errorElement);
    }
};
