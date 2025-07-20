import { checkAuth } from '../api/auth-api.js';
import { UserRole } from '../constants.js';

export const renderHeader = async () => {
    const user = await checkAuth().catch(() => null);
    const isAllowedToCreate = user && (user.role === UserRole.MIDDLE || user.role === UserRole.SENIOR);
    const isAuthenticated = user !== null;

    return `
    <header>
      <nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-dark bg-primary border-bottom box-shadow">
        <div class="container-fluid">
          <a class="navbar-brand me-4" href="index.html">
            Эскалации | WaveSuccess
          </a>
          <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
            <ul class="navbar-nav">
              <li class="nav-item">
                ${isAllowedToCreate ? '<a class="nav-link" href="create-escalation.html">Создать</a>' : ''}
              </li>
              ${isAuthenticated ? `
              <li class="nav-item">
                <a class="nav-link" href="my-responsibilities.html">Мои ответственности</a>
              </li>
              ${isAllowedToCreate ? `
              <li class="nav-item">
                <a class="nav-link" href="my-escalations.html">Мои эскалации</a>
              </li>
              ` : ''}
              ` : ''}
              <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
                  Дополнительно
                </a>
                <ul class="dropdown-menu">
                  ${isAllowedToCreate ? '<li><a class="dropdown-item" href="report.html">Сгенерировать отчёт</a></li>' : ''}
                  <li><a class="dropdown-item" href="roles.html">Информация о ролях</a></li>
                </ul>
              </li>
            </ul>
            <ul class="navbar-nav" id="auth-nav"></ul>
          </div>
        </div>
      </nav>
    </header>
  `;
};