import { AUTH_API_URL } from '../constants.js';

// Вход
export const login = async (email, password) => {
    const response = await fetch(`${AUTH_API_URL}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
        credentials: 'include'
    });

    if (!response.ok) throw new Error("Неверный email или пароль");
    return await response.json();
};

// Регистрация
export const register = async (userData) => {
    const response = await fetch(`${AUTH_API_URL}/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userData),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.title || "Регистрация провалилась");
    }
    return await response.json();
};

// Выход с аккаунта
export const logout = async () => {
    await fetch(`${AUTH_API_URL}/logout`, {
        method: 'POST',
        credentials: 'include'
    });
};

// Проверка авторизации
export const checkAuth = async () => {
    const response = await fetch(`${AUTH_API_URL}/me`, {
        method: 'GET',
        credentials: 'include'
    });
    return response.ok ? await response.json() : null;
};