import {
    getEscalationById,
    updateEscalation
} from '../../api/escalation-api.js';
import {
    getCriteriaByEscalationId,
    createCriteria,
    updateCriteria,
    deleteCriteria
} from '../../api/criteria-api.js';
import {
    getCommentsByEscalationId,
    createComment
} from '../../api/comment.api.js';
import { getUserById } from '../../api/user-api.js';

export class EscalationDetailsService {
    constructor(escalationId) {
        this.escalationId = escalationId;
    }

    // Основные данные
    async getEscalation() {
        const response = await getEscalationById(this.escalationId);
        return response.$values ? response.$values[0] : response;
    }

    async updateEscalationData(data) {
        return await updateEscalation(this.escalationId, data);
    }

    // Критерии
    async getCriteria() {
        const response = await getCriteriaByEscalationId(this.escalationId);
        return response.$values || response || [];
    }

    async createCriterion(description) {
        return await createCriteria(this.escalationId, { description });
    }

    async updateCriterion(criteriaId, data) {
        return await updateCriteria(this.escalationId, criteriaId, data);
    }

    async deleteCriterion(criteriaId) {
        return await deleteCriteria(this.escalationId, criteriaId);
    }

    // Комментарии
    async getComments() {
        const response = await getCommentsByEscalationId(this.escalationId);
        return response.$values || response || [];
    }

    async createComment(text) {
        return await createComment(this.escalationId, text);
    }

    // Пользователи
    async getUser(userId) {
        try {
            return await getUserById(userId);
        } catch {
            return { firstName: 'Аноним', lastName: '' };
        }
    }

    // Вспомогательные методы
    getStatusClass(status) {
        const classes = {
            0: 'bg-secondary', 1: 'bg-warning text-dark',
            2: 'bg-info text-white', 3: 'bg-success', 4: 'bg-danger'
        };
        return classes[status] || 'bg-secondary';
    }

    getStatusText(status) {
        const statusMap = {
            0: 'Новая', 1: 'В работе', 2: 'На проверке',
            3: 'Завершена', 4: 'Отклонена'
        };
        return statusMap[status] || 'Неизвестный статус';
    }
}