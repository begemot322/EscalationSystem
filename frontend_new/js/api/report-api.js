import { REPORT_API_URL } from '../constants/api-urls.js';

export const generateReport = async (reportData) => {
    const response = await fetch(`${REPORT_API_URL}/create`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(reportData),
        credentials: 'include'
    });

    if (!response.ok) {
        let errorMessage = 'Ошибка при генерации документа';

        try {
            const errorData = await response.json();
            errorMessage = errorData.detail || errorMessage;
        } catch {
            if (response.status === 401) {
                errorMessage = 'Вы не авторизированы';
            }
            if (response.status === 403) {
                errorMessage = 'У вас нет прав для генерации отчетов';
            } else if (response.status === 400) {
                errorMessage = 'Неверные параметры запроса';
            }
        }

        throw new Error(errorMessage);
    }

    return await response.blob();
};