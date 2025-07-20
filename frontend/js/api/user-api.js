import { USER_API_URL } from '../constants.js';

export const getUsers = async (filter = {}, pageParams = {}) => {
    const queryParams = new URLSearchParams();

    // Фильтрация по пользователям
    if (filter.firstName) queryParams.append('FirstName', filter.firstName);
    if (filter.lastName) queryParams.append('LastName', filter.lastName);
    if (filter.email) queryParams.append('Email', filter.email);
    if (filter.phoneNumber) queryParams.append('PhoneNumber', filter.phoneNumber);

    // Параметры пагинации
    if (pageParams.page) queryParams.append('Page', pageParams.page);
    if (pageParams.pageSize) queryParams.append('PageSize', pageParams.pageSize);

    const response = await fetch(`${USER_API_URL}?${queryParams.toString()}`, {
        method: 'GET',
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.title || "Не удалось получить список пользователей");
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
        throw new Error(error?.message || "Не удалось получить данные пользователя");
    }

    return await response.json();
};