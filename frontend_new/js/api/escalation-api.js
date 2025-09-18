import { ESCALATION_API_URL } from '../constants/api-urls.js';

// Создание эскалации
export const createEscalation = async (escalationData) => {
    const response = await fetch(ESCALATION_API_URL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(escalationData),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail || errorData.title || "Не удалось создать эскалацию");
    }
    return await response.json();
};

// Получение эскалаций, где пользователь является ответственным
export const getAssignedEscalations = async (page = 1, pageSize = 10) => {
    const response = await fetch(`${ESCALATION_API_URL}/assigned-to-me?page=${page}&pageSize=${pageSize}`, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        throw new Error("Не удалось получить назначенные эскалации");
    }
    return await response.json();
};

// Получение эскалаций, созданных пользователем
export const getAuthoredEscalations = async (page = 1, pageSize = 10) => {
    const response = await fetch(`${ESCALATION_API_URL}/created-by-me?page=${page}&pageSize=${pageSize}`, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        throw new Error("Не удалось получть созданные эскалации");
    }
    return await response.json();
};

// Получить крутые эскалации
export const getFeaturedEscalations = async () => {
    const response = await fetch(`${ESCALATION_API_URL}/featured`, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        throw new Error("Не удалось получить избранные эскалации");
    }

    return await response.json();
};

// Получение эскалации по id
export const getEscalationById = async (id) => {
    const response = await fetch(`${ESCALATION_API_URL}/${id}`, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail || errorData.title || "Эскалация не найдена");
    }
    return await response.json();
};

// Обновление эскалации
export const updateEscalation = async (id, escalationData) => {
    const response = await fetch(`${ESCALATION_API_URL}/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            name: escalationData.name,
            description: escalationData.description,
            status: escalationData.status,
            isFeatured: escalationData.isFeatured
        }),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail || errorData.title || "Не удалось обновить эскалацию");
    }
    return await response.json();
};
