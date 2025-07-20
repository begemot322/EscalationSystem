import { generateReport } from '../api/report-api.js';

export const handleGenerateReport = async () => {
    const form = document.getElementById('report-form');
    const fromDate = form.fromDate.value;
    const toDate = form.toDate.value;
    const status = form.status.value;

    console.log('Form data:', { fromDate, toDate, status }); // Логирование данных формы

    const loadingSpinner = document.getElementById('loading-spinner');
    loadingSpinner.classList.remove('d-none');

    const oldError = document.querySelector('.report-error');
    if (oldError) oldError.remove();

    try {
        // Валидация дат
        if (!fromDate || !toDate) {
            throw new Error('Необходимо указать обе даты');
        }
        if (new Date(fromDate) > new Date(toDate)) {
            throw new Error('Дата начала не может быть позже даты окончания');
        }

        const reportData = {
            fromDate,
            toDate,
            status: status === '' ? null : parseInt(status)
        };

        const blob = await generateReport(reportData);

        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Отчет_${new Date().toISOString().slice(0, 19).replace(/[:T]/g, '_')}.pdf`;
        document.body.appendChild(a);
        a.click();

        // Даем время для скачивания перед удалением
        setTimeout(() => {
            a.remove();
            window.URL.revokeObjectURL(url);
        }, 100);
    } catch (error) {
        console.error('Error generating report:', error); // Логирование ошибки
        const errorElement = document.createElement('div');
        errorElement.className = 'report-error text-danger mt-3 text-center';
        errorElement.textContent = error.message || 'Произошла неизвестная ошибка';
        form.after(errorElement);
    } finally {
        loadingSpinner.classList.add('d-none');
    }
};
