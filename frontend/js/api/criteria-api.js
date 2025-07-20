import { CRITERIA_API_URL } from '../constants.js';

export const getCriteriaByEscalationId = async (escalationId) => {
    const url = `${CRITERIA_API_URL}/escalation/${escalationId}/criteria`;
    const response = await fetch(url, {
        method: 'GET',
        credentials: 'include'
    });

    console.log(`GET ${url} — status:`, response.status);

    if (!response.ok) {
        const errorText = await response.text();
        console.error("Ошибка получения критериев:", errorText);
        throw new Error("Не удалось получить критерии");
    }

    const data = await response.json();
    console.log("Критерии получены:", data);
    return data;
};

export const updateCriteria = async (escalationId, criteriaId, dto) => {
    const response = await fetch(`${CRITERIA_API_URL}/escalation/${escalationId}/criteria/${criteriaId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(dto),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.title || "Не удалось обновить критерий");
    }
    return await response.json();
};
export const createCriteria = async (escalationId, criteriaData) => {
    const response = await fetch(`${CRITERIA_API_URL}/escalation/${escalationId}/criteria`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(criteriaData),
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.title || "Не удалось создать критерий");
    }
    return await response.json();
};

export const deleteCriteria = async (escalationId, criteriaId) => {
    const response = await fetch(`${CRITERIA_API_URL}/escalation/${escalationId}/criteria/${criteriaId}`, {
        method: 'DELETE',
        credentials: 'include'
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.title || "Не удалось удалить критерий");
    }
    return true;
};