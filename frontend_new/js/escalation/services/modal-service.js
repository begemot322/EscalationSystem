export class ModalService {
    static createModal(title, bodyHtml, onSave, onClose = null) {
        const modalId = 'modal-' + Date.now();

        const modalHtml = `
            <div class="modal fade" id="${modalId}" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">${title}</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            ${bodyHtml}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
                            <button type="button" class="btn btn-primary" id="modal-save-btn" data-bs-dismiss="modal">Сохранить</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        const modalContainer = document.createElement('div');
        modalContainer.innerHTML = modalHtml;
        document.body.appendChild(modalContainer);

        const modal = new bootstrap.Modal(document.getElementById(modalId));

        document.getElementById('modal-save-btn').addEventListener('click', async () => {
            await onSave();
        });

        modal._element.addEventListener('hidden.bs.modal', () => {
            if (onClose) onClose();
            modalContainer.remove();
        });

        modal.show();
        return modal;
    }

    static createCriteriaModal(criterion, onSave) {
        const bodyHtml = `
            <div class="mb-3">
                <label class="form-label">Описание</label>
                <textarea class="form-control" id="modal-criteria-description" rows="3">${criterion.description}</textarea>
            </div>
            <div class="form-check">
                <input class="form-check-input" type="checkbox" id="modal-criteria-completed" ${criterion.isCompleted ? 'checked' : ''}>
                <label class="form-check-label">Выполнено</label>
            </div>
        `;

        return this.createModal(
            'Редактировать критерий',
            bodyHtml,
            async () => {
                const description = document.getElementById('modal-criteria-description').value.trim();
                const isCompleted = document.getElementById('modal-criteria-completed').checked;

                await onSave({ description, isCompleted });
            }
        );
    }

    static createEscalationModal(escalation, onSave) {
        const bodyHtml = `
        <div class="mb-3">
            <label class="form-label">Название</label>
            <input type="text" class="form-control" id="modal-escalation-name" value="${escalation.name}" required>
        </div>
        <div class="mb-3">
            <label class="form-label">Описание</label>
            <textarea class="form-control" id="modal-escalation-description" rows="3">${escalation.description || ''}</textarea>
        </div>
        <div class="mb-3">
            <label class="form-label">Статус</label>
            <select class="form-select" id="modal-escalation-status">
                <option value="0" ${escalation.status === 0 ? 'selected' : ''}>Новая</option>
                <option value="1" ${escalation.status === 1 ? 'selected' : ''}>В работе</option>
                <option value="2" ${escalation.status === 2 ? 'selected' : ''}>На проверке</option>
                <option value="3" ${escalation.status === 3 ? 'selected' : ''}>Завершена</option>
                <option value="4" ${escalation.status === 4 ? 'selected' : ''}>Отклонена</option>
            </select>
        </div>
        <div class="mb-3">
            <div class="form-check form-switch">
                <input class="form-check-input" type="checkbox" id="modal-escalation-featured" 
                    ${escalation.isFeatured ? 'checked' : ''}>
                <label class="form-check-label" for="modal-escalation-featured">
                    Избранная эскалация
                </label>
            </div>
            <div class="form-text">Избранные эскалации помечаются золотой звездой</div>
        </div>
    `;

        return this.createModal(
            'Редактировать эскалацию',
            bodyHtml,
            async () => {
                const name = document.getElementById('modal-escalation-name').value.trim();
                const description = document.getElementById('modal-escalation-description').value.trim();
                const status = parseInt(document.getElementById('modal-escalation-status').value);
                const isFeatured = document.getElementById('modal-escalation-featured').checked;


                await onSave({ name, description, status, isFeatured });
            }
        );
    }
}