import { getUsers } from '../../api/user-api.js';
import { UserRole } from '../../constants/user-roles.js';
import { Pagination } from '../../utils/pagination.js';

export class UsersService {
    constructor(pageSize = 10) {
        this.pagination = new Pagination(pageSize);
        this.pagination.onPageChange = this.loadUsers.bind(this);
    }
gj
    async loadUsers(page = 1, filter = {}) {
        try {
            // Подготавливаем параметры в формате, который ожидает бэкенд
            const pageParams = {
                page: page,
                pageSize: this.pagination.pageSize
            };

            const usersResponse = await getUsers(filter, pageParams);

            // Обновляем пагинацию (предполагаем, что ответ содержит totalCount)
            this.pagination.updateTotalItems(usersResponse.totalCount || usersResponse.items.length);
            this.pagination.currentPage = page;

            return {
                users: usersResponse.items || [],
                totalCount: usersResponse.totalCount || 0,
                page: page,
                pageSize: this.pagination.pageSize,
                paginationInfo: this.pagination.getPaginationInfo(),
                hasNext: this.pagination.hasNext,
                hasPrev: this.pagination.hasPrev
            };

        } catch (error) {
            throw new Error('Не удалось загрузить пользователей');
        }
    }

    async searchUsers(searchTerm) {
        const filter = searchTerm ? {
            firstName: searchTerm,
        } : {};

        return await this.pagination.search(filter);
    }

    async nextPage()  {
        return await this.pagination.next();
    }

    async prevPage() {
        return await this.pagination.prev();
    }

    getRoleName(role) {
        switch (role) {
            case UserRole.JUNIOR: return 'Junior';
            case UserRole.MIDDLE: return 'Middle';
            case UserRole.SENIOR: return 'Senior';
            default: return 'Unknown';
        }
    }

    formatUserOption(user, index) {
        const globalIndex = ((this.pagination.currentPage - 1) * this.pagination.pageSize) + index + 1;
        return `${globalIndex}. ${user.firstName} ${user.lastName} (${this.getRoleName(user.role)})`;
    }
}