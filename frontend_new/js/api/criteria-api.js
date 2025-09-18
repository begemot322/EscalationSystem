import { ESCALATION_API_URL } from '../constants/api-urls.js';

export const getCriteriaByEscalationId = async (escalationId) => {
    const url = `${ESCALATION_API_URL}/${escalationId}/criteria`;
    const response = await fetch(url, {
        method: 'GET',
        credentials: 'include'
    });


    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText.detail || errorText.title || "Не удалось получить критерии");
    }

    return  await response.json();
};

export const updateCriteria = async (escalationId, criteriaId, dto) => {
    const response = await fetch(`${ESCALATION_API_URL}/${escalationId}/criteria/${criteriaId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(dto),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail ||  errorData.title || "Не удалось обновить критерий");
    }
    return await response.json();
};

export const createCriteria = async (escalationId, criteriaData) => {
    const response = await fetch(`${ESCALATION_API_URL}/${escalationId}/criteria`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(criteriaData),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail ||errorData.title|| "Не удалось создать критерий");
    }
    return await response.json();
};

export const deleteCriteria = async (escalationId, criteriaId) => {
    const response = await fetch(`${ESCALATION_API_URL}/${escalationId}/criteria/${criteriaId}`, {
        method: 'DELETE',
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.detail || errorData.title || "Не удалось удалить критерий");
    }
    return true;
};