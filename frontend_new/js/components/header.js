import { checkAuth } from '../api/auth-api.js';
import { UserRole } from '../constants/user-roles.js';

export const renderHeader = async () => {
    const user = await checkAuth();
    const userRole = user?.role;
    const isAuthenticated = user !== null;

    //  видимость пунктов меню
    const showCreate = userRole === UserRole.MIDDLE || userRole === UserRole.SENIOR;
    const showResponsibilities = isAuthenticated;
    const showMyEscalations = userRole === UserRole.MIDDLE || userRole === UserRole.SENIOR;
    const showReport = userRole === UserRole.MIDDLE || userRole === UserRole.SENIOR;
    const showAdditionalMenu = isAuthenticated;

    return `
    <header>
      <nav class="navbar navbar-expand-lg bg-dark" data-bs-theme="dark">
        <div class="container-fluid">
          <a class="navbar-brand me-4" href="index.html">
            Эскалации | WaveForm
          </a>
          <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarColor02">
            <span class="navbar-toggler-icon"></span>
          </button>
          <div class="collapse navbar-collapse" id="navbarColor02">
            <ul class="navbar-nav me-auto">
              ${showCreate ? `
              <li class="nav-item">
                <a class="nav-link" href="create-escalation.html">Создать</a>
              </li>
              ` : ''}
              ${showResponsibilities ? `
              <li class="nav-item">
                <a class="nav-link" href="my-responsibilities.html">Мои ответственности</a>
              </li>
              ` : ''}
              ${showMyEscalations ? `
              <li class="nav-item">
                <a class="nav-link" href="my-escalations.html">Мои эскалации</a>
              </li>
              ` : ''}
              ${showAdditionalMenu ? `
              <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
                  Дополнительно
                </a>
                <ul class="dropdown-menu">
                  ${showReport ? `
                  <li><a class="dropdown-item" href="report.html">Сгенерировать отчёт</a></li>
                  ` : ''}
                  <li><a class="dropdown-item" href="roles.html">Информация о ролях</a></li>
                </ul>
              </li>
              ` : ''}
            </ul>
            <ul class="navbar-nav ms-auto" id="auth-nav">
            </ul>
          </div>
        </div>
      </nav>
    </header>
  `;
};