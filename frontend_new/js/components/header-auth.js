import { checkAuth, logout } from '../api/auth-api.js';

export const initHeaderAuth = async () => {
    const authNav = document.getElementById('auth-nav');

    const user = await checkAuth();

    if (user) {

        authNav.innerHTML = `
            <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
                    <i class="bi bi-person-circle"></i> ${user.firstName} ${user.lastName} 
                </a>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li><a class="dropdown-item" href="#">Профиль</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li>
                        <button class="dropdown-item" id="logout-btn">
                            <i class="bi bi-box-arrow-right"></i> Выйти
                        </button>
                    </li>
                </ul>
            </li>
        `;

        document.getElementById('logout-btn').addEventListener('click', async () => {
            await logout();
            window.location.href = 'index.html';
        });
    } else {
        authNav.innerHTML = `
            <li class="nav-item">
                <a class="nav-link" href="login.html">Войти</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="register.html">Зарегистрироваться</a>
            </li>
        `;
    }
};