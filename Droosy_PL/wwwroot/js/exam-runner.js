(() => {
 'use strict';
 const form=document.querySelector('[data-exam-form]');if(!form)return;
 const cards=[...form.querySelectorAll('[data-exam-question]')],jumps=[...form.querySelectorAll('[data-question-jump]')];
 const previous=form.querySelector('[data-question-prev]'),next=form.querySelector('[data-question-next]'),number=form.querySelector('[data-question-number]');
 const clock=form.querySelector('[data-exam-time]'),status=form.querySelector('[data-exam-save-state]'),submit=form.querySelector('[data-exam-submit]'),manualSave=form.querySelector('[data-exam-save]'),dialog=form.querySelector('[data-exam-confirm]');
 const deadline=Date.parse(form.dataset.deadline);let offset=Date.parse(form.dataset.serverNow)-Date.now(),index=0,busy=false,queued=false,dirty=false,finished=false;
 function seconds(){return Math.max(0,Math.ceil((deadline-(Date.now()+offset))/1000));}
 function update(){cards.forEach((card,i)=>{card.hidden=i!==index;const answered=!!card.querySelector('input:checked');jumps[i]?.classList.toggle('answered',answered);jumps[i]?.classList.toggle('current',i===index);});previous.disabled=index===0;next.disabled=index===cards.length-1;number.textContent='السؤال '+(index+1)+' من '+cards.length;}
 previous?.addEventListener('click',()=>{index=Math.max(0,index-1);update();});next?.addEventListener('click',()=>{index=Math.min(cards.length-1,index+1);update();});jumps.forEach((button,i)=>button.addEventListener('click',()=>{index=i;update();}));
 async function save(intent='save'){
  if(finished)return;
  if(busy){queued=intent==='submit'?'submit':queued||'save';return;}
  busy=true;status.textContent='جارٍ حفظ الإجابات…';
  const data=new FormData(form);data.set('intent',intent);
  try{
   const response=await fetch(form.action,{method:'POST',body:data,headers:{Accept:'application/json'},credentials:'same-origin',keepalive:intent==='save'});
   if(!response.ok)throw Error('save failed');const result=await response.json();offset=Date.parse(result.serverNowUtc)-Date.now();dirty=false;
   status.textContent=result.submitted?'تم تسليم الاختبار':'✓ الإجابات محفوظة';
   if(result.submitted){finished=true;location.assign(location.pathname);return;}
  }catch{status.textContent='تعذّر الحفظ. اضغط حفظ الإجابات لإعادة المحاولة.';dirty=true;}
  finally{busy=false;if(queued){const intent=queued;queued=false;save(intent);}}
 }
 form.addEventListener('change',event=>{if(event.target.matches('input[type="radio"]')){dirty=true;update();save();}});
 form.addEventListener('submit',event=>{
  event.preventDefault();
  if(event.submitter===manualSave){save();return;}
  if(seconds()===0){save('submit');return;}
  if(dialog?.showModal)dialog.showModal();else if(window.confirm('هل تريد تسليم الاختبار؟ لا يمكن تعديل الإجابات بعد التسليم.'))save('submit');
 });
 form.querySelector('[data-confirm-submit]')?.addEventListener('click',()=>{dialog.close();save('submit');});form.querySelector('[data-cancel-submit]')?.addEventListener('click',()=>dialog.close());
 function tick(){const remaining=seconds();clock.textContent=Math.floor(remaining/60).toString().padStart(2,'0')+':'+(remaining%60).toString().padStart(2,'0');form.classList.toggle('time-warning',remaining<=60);if(remaining===0&&!finished){form.querySelectorAll('input[type="radio"]').forEach(input=>input.disabled=true);submit.textContent='تأكيد انتهاء الاختبار';save('submit');}}
 document.addEventListener('visibilitychange',()=>{if(document.visibilityState==='hidden'&&dirty)save();});
 window.addEventListener('pagehide',()=>{if(dirty){const data=new FormData(form);data.set('intent','save');fetch(form.action,{method:'POST',body:data,credentials:'same-origin',headers:{Accept:'application/json'},keepalive:true}).catch(()=>{});}});
 setInterval(()=>{if(dirty&&!finished)save();},10000);setInterval(tick,1000);update();tick();
})();
