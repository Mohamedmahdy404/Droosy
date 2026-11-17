(() => {
    'use strict';
    const generation = document.querySelector('meta[name="droosy-generation"]')?.content || '';
    const key = 'droosy-hints:' + generation + ':' + location.pathname + location.search;
    const hints = [...document.querySelectorAll('[data-hint]')];
    let timer;
    function hide() {
        hints.forEach(hint => {
            if (hint.contains(document.activeElement)) {
                hint.addEventListener('focusout', () => { hint.hidden = true; }, { once: true });
            } else hint.hidden = true;
        });
    }
    function show() {
        clearTimeout(timer);
        hints.forEach(hint => { hint.hidden = false; });
        timer = setTimeout(hide, 11000);
    }
    let seen = false;
    try {
        seen = sessionStorage.getItem(key) === 'seen';
        sessionStorage.setItem(key, 'seen');
    } catch { /* Storage is optional for these demo hints. */ }
    hints.forEach(hint => { hint.hidden = seen; });
    if (!seen) show();
    document.querySelector('[data-show-hints]')?.addEventListener('click', show);
    document.querySelectorAll('[data-dismiss]').forEach(button => {
        button.addEventListener('click', () => {
            if (button.parentElement.contains(document.activeElement)) {
                const next = button.parentElement.parentElement.querySelector('a, button[type="submit"]');
                (next || document.querySelector('[data-show-hints]'))?.focus({ preventScroll: true });
            }
            button.parentElement.hidden = true;
        });
    });
    const status = document.querySelector('#publication-status');
    const schedule = document.querySelector('#schedule-field');
    function update() {
        if (!status || !schedule) return;
        const scheduled = status.value === 'Scheduled';
        schedule.hidden = !scheduled;
        const input = schedule.querySelector('input');
        if (input) {
            input.required = scheduled;
            input.disabled = !scheduled;
        }
    }
    status?.addEventListener('change', update);
    update();
    document.querySelectorAll('form').forEach(form => {
        if (form.matches?.('[data-exam-form]')) return;
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) return;
            const button = event?.submitter || form.querySelector('button[type="submit"]');
            if (button) {
                if (button.getAttribute?.('aria-busy') === 'true') { event?.preventDefault(); return; }
                if (!button.name) button.disabled = true;
                button.setAttribute('aria-busy', 'true');
            }
        });
    });
    // Mobile back navigation can restore the disabled button from the page cache.
    window.addEventListener('pageshow', () => {
        document.querySelectorAll('button[aria-busy="true"]').forEach(button => {
            button.disabled = false;
            button.removeAttribute('aria-busy');
        });
    });
})();

