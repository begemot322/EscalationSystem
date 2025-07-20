import { REPORT_API_URL } from '../constants.js';

export const generateReport = async (reportData) => {
    try {
        const response = await fetch(`${REPORT_API_URL}/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(reportData),
            credentials: 'include'
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || 'Ошибка при генерации отчета');
        }

        return await response.blob();
    } catch (error) {
        throw new Error('У вас нет прав для этого');
    }
};