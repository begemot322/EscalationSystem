import { COMMENT_API_URL } from '../constants.js';

export const getCommentsByEscalationId = async (escalationId) => {
    const response = await fetch(`${COMMENT_API_URL}/${escalationId}/comment`, {
        method: 'GET',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
        }
    });

    if (!response.ok) {
        throw new Error('Ошибка при получении комментариев');
    }

    return await response.json();
};

export const createComment = async (escalationId, text) => {
    const response = await fetch(`${COMMENT_API_URL}/${escalationId}/comment`, {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Text: text }),
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.message || 'Не удалось создать комментарий');
    }

    return await response.json();
};