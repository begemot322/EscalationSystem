import {
    getAssignedEscalations,
    getAuthoredEscalations
} from '../api/escalation-api.js';

let currentPage = 1;
const pageSize = 10;
let totalPages = 1;
let currentType = '';

export const handleAssignedEscalations = async (page = 1) => {
    currentPage = page;
    currentType = 'assigned';
    try {
        const data = await getAssignedEscalations(currentPage, pageSize);
        renderEscalations(data.items, data, true); // true - это isAssigned
    } catch (error) {
        showError(error.message);
    }
};

export const handleAuthoredEscalations = async (page = 1) => {
    currentPage = page;
    currentType = 'authored';
    try {
        const data = await getAuthoredEscalations(currentPage, pageSize);
        renderEscalations(data.items, data, false); // false - не isAssigned
    } catch (error) {
        showError(error.message);
    }
};

function renderEscalations(escalationsRaw, paginationData, isAssigned = false) {
    const container = document.getElementById('escalations-container');
    if (!container) return;

    container.innerHTML = '';

    const escalations = escalationsRaw?.$values ?? escalationsRaw ?? [];

    if (!escalations.length) {
        container.innerHTML = `<div class="alert alert-info">${isAssigned ? 'На вас не назначено эскалаций' : 'Вы не создали ни одной эскалации'}</div>`;
        return;
    }

    totalPages = Math.ceil(paginationData.totalCount / pageSize);
    updatePageCounter(currentPage, totalPages);

    const list = document.createElement('div');
    list.className = 'list-group mb-4';

    escalations.forEach(esc => {
        const item = document.createElement('a');
        item.href = `escalation-details.html?id=${esc.id}&view=${isAssigned ? 'comment' : 'edit'}`;
        item.className = 'list-group-item list-group-item-action';
        item.innerHTML = `
            <div class="d-flex justify-content-between">
                <h5>${esc.name}</h5>
                <span class="badge ${getStatusClass(esc.status)}">
                    ${getStatusText(esc.status)}
                </span>
            </div>
            <p class="mb-1">${esc.description || 'Без описания'}</p>
            <small>Создано: ${new Date(esc.createdAt).toLocaleDateString()}</small>
        `;
        list.appendChild(item);
    });

    container.appendChild(list);
    renderPagination(container);
}

function updatePageCounter(current, total) {
    const currentEl = document.getElementById('current-page');
    const totalEl = document.getElementById('total-pages');
    if (currentEl) currentEl.textContent = current;
    if (totalEl) totalEl.textContent = total;
}

function renderPagination(container) {
    const paginationContainer = document.createElement('div');
    paginationContainer.className = 'd-flex justify-content-center';

    const pagination = document.createElement('ul');
    pagination.className = 'pagination';

    // Previous button
    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
    prevLi.innerHTML = `<a class="page-link" href="#">&laquo;</a>`;
    prevLi.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage > 1) loadPage(currentPage - 1);
    });
    pagination.appendChild(prevLi);

    for (let i = 1; i <= totalPages; i++) {
        const pageLi = document.createElement('li');
        pageLi.className = `page-item ${i === currentPage ? 'active' : ''}`;
        pageLi.innerHTML = `<a class="page-link" href="#">${i}</a>`;
        pageLi.addEventListener('click', (e) => {
            e.preventDefault();
            loadPage(i);
        });
        pagination.appendChild(pageLi);
    }

    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${currentPage >= totalPages ? 'disabled' : ''}`;
    nextLi.innerHTML = `<a class="page-link" href="#">&raquo;</a>`;
    nextLi.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage < totalPages) loadPage(currentPage + 1);
    });
    pagination.appendChild(nextLi);

    paginationContainer.appendChild(pagination);
    container.appendChild(paginationContainer);
}

function loadPage(page) {
    if (currentType === 'assigned') {
        handleAssignedEscalations(page);
    } else {
        handleAuthoredEscalations(page);
    }
    window.scrollTo(0, 0);
}

function showError(message) {
    const container = document.getElementById('escalations-container') || document.body;
    const errorElement = document.createElement('div');
    errorElement.className = 'alert alert-danger';
    errorElement.textContent = message;
    container.innerHTML = '';
    container.appendChild(errorElement);
}

function getStatusClass(status) {
    if (typeof status === 'number') {
        const classes = {
            0: 'bg-secondary',
            1: 'bg-warning text-dark',
            2: 'bg-info text-white',
            3: 'bg-success',
            4: 'bg-danger',
        };
        return classes[status] || 'bg-secondary';
    }
}
function getStatusText(status) {
    if (typeof status === 'number') {
        const statusMap = {
            0: 'Новая',
            1: 'В работе',
            2: 'На проверке',
            3: 'Завершена',
            4: 'Отклонена'
        };
        return statusMap[status] || 'Неизвестный статус';
    }
}