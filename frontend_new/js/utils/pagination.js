export class Pagination {
    constructor(pageSize = 10) {
        this.currentPage = 1;
        this.pageSize = pageSize;
        this.totalItems = 0;
        this.filter = {};
        this.onPageChange = null;
    }

    get totalPages() {
        return Math.ceil(this.totalItems / this.pageSize);
    }

    get hasNext() {
        return this.currentPage < this.totalPages;
    }

    get hasPrev() {
        return this.currentPage > 1;
    }

    async next() {
        if (this.hasNext) {
            this.currentPage++;
            return await this.loadPage();
        }
        return null;
    }

    async prev() {
        if (this.hasPrev) {
            this.currentPage--;
            return await this.loadPage();
        }
        return null;
    }

    async goToPage(page) {
        if (page >= 1 && page <= this.totalPages) {
            this.currentPage = page;
            return await this.loadPage();
        }
        return null;
    }

    async search(newFilter) {
        this.currentPage = 1;
        this.filter = newFilter;
        return await this.loadPage();
    }

    async loadPage() {
        if (this.onPageChange) {
            const result = await this.onPageChange(this.currentPage, this.filter);
            // Обновляем totalItems из ответа сервера
            if (result && result.totalCount !== undefined) {
                this.updateTotalItems(result.totalCount);
            }
            return result;
        }
        return null;
    }

    updateTotalItems(total) {
        this.totalItems = total;
    }

    getPaginationInfo() {
        if (this.totalItems === 0) {
            return 'Нет данных';
        }

        const start = ((this.currentPage - 1) * this.pageSize) + 1;
        const end = Math.min(this.currentPage * this.pageSize, this.totalItems);
        return `Показано ${start}-${end} из ${this.totalItems}`;
    }

}