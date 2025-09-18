import { USER_API_URL } from '../constants/api-urls.js';

export const getUsers = async (filter = {}, pageParams = {}) => {
    // Создаем URL с параметрами
    const url = new URL(USER_API_URL);

    // Добавляем параметры фильтрации
    Object.keys(filter).forEach(key => {
        if (filter[key] !== null && filter[key] !== undefined && filter[key] !== '') {
            url.searchParams.append(key, filter[key]);
        }
    });

    // Добавляем параметры пагинации в формате PageParams
    if (pageParams.page) {
        url.searchParams.append('Page', pageParams.page);
    }
    if (pageParams.pageSize) {
        url.searchParams.append('PageSize', pageParams.pageSize);
    }

    const response = await fetch(url, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail || errorData.title || "Не удалось получить пользователей");
    }

    return await response.json();
};

export const getUserById = async (userId) => {
    const response = await fetch(`${USER_API_URL}/${userId}`, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error.detail || error.title || "Не удалось получить данные пользователя");
    }

    return await response.json();
};