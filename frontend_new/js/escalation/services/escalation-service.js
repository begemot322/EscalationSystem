import { getAuthoredEscalations, getAssignedEscalations } from '../../api/escalation-api.js';
import { Pagination } from '../../utils/pagination.js';

export class EscalationService {
    constructor(pageSize = 10) {
        this.pagination = new Pagination(pageSize);
        this.pagination.onPageChange = this.loadEscalations.bind(this);
        this.currentType = 'authored'; // По умолчанию созданные
    }

    async loadEscalations(page = 1, filter = {}) {
        try {
            let response;

            if (this.currentType === 'authored') {
                response = await getAuthoredEscalations(page, this.pagination.pageSize);
            } else {
                response = await getAssignedEscalations(page, this.pagination.pageSize);
            }

            this.pagination.updateTotalItems(response.totalCount);
            this.pagination.currentPage = page;

            return {
                escalations: response.items || [],
                totalCount: response.totalCount || 0,
                page: page,
                pageSize: this.pagination.pageSize,
                paginationInfo: this.pagination.getPaginationInfo(),
                hasNext: this.pagination.hasNext,
                hasPrev: this.pagination.hasPrev,
                type: this.currentType
            };

        } catch (error) {
            throw new Error('Не удалось загрузить эскалации');
        }
    }

    // Методы для смены типа эскалаций
    async showAuthored() {
        this.currentType = 'authored';
        this.pagination.currentPage = 1;
        return await this.loadEscalations(1);
    }

    async showAssigned() {
        this.currentType = 'assigned';
        this.pagination.currentPage = 1;
        return await this.loadEscalations(1);
    }

    async nextPage() {
        return await this.pagination.next();
    }

    async prevPage() {
        return await this.pagination.prev();
    }

    getStatusClass(status) {
        const classes = {
            0: 'bg-secondary',
            1: 'bg-warning text-dark',
            2: 'bg-info text-white',
            3: 'bg-success',
            4: 'bg-danger',
        };
        return classes[status] || 'bg-secondary';
    }

    getStatusText(status) {
        const statusMap = {
            0: 'Новая',
            1: 'В работе',
            2: 'На проверке',
            3: 'Завершена',
            4: 'Отклонена'
        };
        return statusMap[status] || 'Неизвестный статус';
    }

    getTypeText() {
        return this.currentType === 'authored' ? 'созданные' : 'назначенные';
    }
}