import { generateReport } from '../api/report-api.js';
import { showError, showSuccess, clearMessages } from '../utils/message-handler.js';

export const handleGenerateReport = async () => {
    const form = document.getElementById('report-form');
    const messageContainer = document.getElementById('message-container');
    const submitButton = form.querySelector('button[type="submit"]');

    const formData = {
        fromDate: form.fromDate.value,
        toDate: form.toDate.value,
        status: form.status.value ? parseInt(form.status.value) : null
    };

    // Валидация дат
    if (!formData.fromDate || !formData.toDate) {
        showError(messageContainer, 'Пожалуйста, выберите обе даты');
        return;
    }

    if (new Date(formData.fromDate) > new Date(formData.toDate)) {
        showError(messageContainer, 'Дата начала не может быть позже даты окончания');
        return;
    }

    clearMessages(messageContainer);

    try {
        // Меняем текст кнопки на время генерации
        const originalText = submitButton.innerHTML;
        submitButton.disabled = true;
        submitButton.innerHTML = '<i class="bi bi-file-earmark-pdf me-2"></i>Генерация...';

        // Генерируем отчет
        const blob = await generateReport(formData);

        // Создаем ссылку для скачивания
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = url;
        a.download = `Отчет_${formData.fromDate}_${formData.toDate}.pdf`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);

        showSuccess(messageContainer, 'Отчет успешно сгенерирован и скачан!');

    } catch (error) {
        showError(messageContainer, error.message || 'Ошибка при генерации отчета');
    } finally {
        // Возвращаем кнопку в исходное состояние
        submitButton.disabled = false;
        submitButton.innerHTML = '<i class="bi bi-file-earmark-pdf me-2"></i>Сгенерировать отчёт';
    }
};

// Инициализация формы
export const initReportForm = () => {
    const form = document.getElementById('report-form');
    if (form) {
        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            await handleGenerateReport();
        });

        // Установка дефолтных дат (последние 30 дней)
        const today = new Date();
        const thirtyDaysAgo = new Date();
        thirtyDaysAgo.setDate(today.getDate() - 30);

        form.fromDate.value = thirtyDaysAgo.toISOString().split('T')[0];
        form.toDate.value = today.toISOString().split('T')[0];
    }
};