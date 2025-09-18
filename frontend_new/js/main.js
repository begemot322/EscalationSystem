import { renderHeader } from './components/header.js';
import { renderFooter } from './components/footer.js';
import { initHeaderAuth } from './components/header-auth.js';


export const initApp = async () => {

    document.body.insertAdjacentHTML('afterbegin', await renderHeader());
    document.body.insertAdjacentHTML('beforeend', await renderFooter());

    await initHeaderAuth();
};