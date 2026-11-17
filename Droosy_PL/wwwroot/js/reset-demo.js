(() => {
    'use strict';
    const open = document.querySelector('[data-open-reset]');
    const dialog = document.querySelector('[data-reset-dialog]');
    if (!open || !dialog) return;
    open.addEventListener('click', () => {
        dialog.showModal();
        // Focus the reversible choice; Enter must not accidentally erase the experience.
        dialog.querySelector('form [data-cancel-reset]').focus();
    });
    dialog.querySelectorAll('[data-cancel-reset]').forEach(button => {
        button.addEventListener('click', () => dialog.close());
    });
    dialog.addEventListener('close', () => open.focus({ preventScroll: true }));
})();
