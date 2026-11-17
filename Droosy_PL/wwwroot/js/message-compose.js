(() => {
 'use strict';
 const input=document.querySelector('[data-message-file]'),label=document.querySelector('[data-file-label]');
 const t=text=>window.DroosyI18n?.t(text)||text;
 function update(){
  if(!input)return;
  const file=input.files?.[0],remaining=Number(input.dataset.remainingBytes??10*1024*1024);
  if(label)label.textContent=file?t('المرفق: ')+file.name:'';
  const error=file&&file.size>2*1024*1024?'الحد الأقصى لمرفق الرسالة 2 MB.':file&&file.size>remaining?'وصلت إلى مساحة مرفقات الشات المتاحة لتجربتك: 10 MB.':'';
  input.setCustomValidity(t(error));if(error)input.reportValidity();
 }
 input?.addEventListener('change',update);
 document.addEventListener('droosy-language-change',update);
})();
