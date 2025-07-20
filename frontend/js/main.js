import { renderHeader } from './components/header.js';
import { renderFooter } from './components/footer.js';
import { AuthUI } from './auth/auth-ui.js';
import { initUiHelpers } from './utils/ui-helpers.js';

export const initApp =  async () => {

    document.body.insertAdjacentHTML('afterbegin', await renderHeader());
    document.body.insertAdjacentHTML('beforeend', renderFooter());

    const container = document.querySelector('.container');
    if (container) {
        container.classList.add('page-transition');
    }
   await AuthUI();

};
