(() => {
    'use strict';
    const video = document.querySelector('[data-lesson-video]');
    if (!video) return;
    const state = document.querySelector('[data-video-state]');
    const message = document.querySelector('[data-video-message]');
    const saved = document.querySelector('[data-video-save-state]');
    const retry = document.querySelector('[data-retry-progress]');
    const meter = document.querySelector('[data-video-meter]');
    const bar = document.querySelector('[data-video-bar]');
    let saving = false, queued = false, lastSave = 0;
    const payload = () => ({ position: video.currentTime, played: Array.from({length:video.played.length}, (_,i) => [video.played.start(i), video.played.end(i)]) });
    async function save() {
        if (saving) { queued = true; return; }
        if (!video.played.length) return;
        saving = true; saved.textContent = 'جارٍ حفظ تقدمك…';
        try {
            const response = await fetch(video.dataset.saveUrl, { method:'POST', headers:{'Content-Type':'application/json','X-Droosy-Generation':document.querySelector('meta[name="droosy-generation"]')?.content || ''}, body:JSON.stringify(payload()), credentials:'same-origin' });
            if (!response.ok) throw new Error('save failed');
            const result = await response.json();
            const percent = Math.min(100, Math.floor(result.watchedSeconds / video.duration * 100));
            bar.style.width = percent + '%'; meter.setAttribute('aria-valuenow', percent);
            state.textContent = result.completed ? 'مكتمل' : 'بدأ التعلم';
            message.textContent = result.completed ? '✓ أكملت الدرس! تقدمك محفوظ ويظهر عند ولي الأمر.' : 'شاهدت ' + percent + '٪. يكتمل الدرس بعد مشاهدة ٩٠٪ فعليًا.';
            saved.textContent = '✓ تم حفظ تقدمك'; retry.hidden = true; lastSave = Date.now();
        } catch { saved.textContent = 'تعذّر الحفظ الآن. أعد المحاولة قبل مغادرة الصفحة.'; retry.hidden = false; }
        finally { saving = false; if (queued) { queued = false; save(); } }
    }
    video.addEventListener('loadedmetadata', () => {
        const position = Number(video.dataset.position || 0);
        if (position > 0 && position < video.duration - .5) video.currentTime = position;
    }, {once:true});
    video.addEventListener('timeupdate', () => { if (!video.paused && Date.now() - lastSave > 5000) { lastSave = Date.now(); save(); } });
    video.addEventListener('pause', save); video.addEventListener('ended', save);
    retry?.addEventListener('click', save);
    document.addEventListener('visibilitychange', () => { if (document.visibilityState === 'hidden') save(); });
    window.addEventListener('pagehide', () => { if (video.played.length) fetch(video.dataset.saveUrl,{method:'POST',headers:{'Content-Type':'application/json','X-Droosy-Generation':document.querySelector('meta[name="droosy-generation"]')?.content || ''},body:JSON.stringify(payload()),credentials:'same-origin',keepalive:true}).catch(()=>{}); });
})();

