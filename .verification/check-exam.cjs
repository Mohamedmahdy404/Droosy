const fs=require('node:fs'),vm=require('node:vm'),assert=require('node:assert/strict');
const code=fs.readFileSync('Droosy_PL/wwwroot/js/exam-runner.js','utf8');
const flush=()=>new Promise(r=>setImmediate(r));
function page(expired=false,fail=false){
 const events={},intervals=[],requests=[],radios=[{disabled:false,value:'choice-1'}];
 const node=()=>({disabled:false,textContent:'',classList:{toggle(){}},addEventListener(name,fn){this[name]=fn;}});
 const controls=Object.fromEntries(['prev','next','number','time','save-state','submit','save','confirm'].map(x=>[x,node()]));
 const cards=[{hidden:false,querySelector:()=>radios[0]}],jump=node();
 const form={dataset:{deadline:new Date(Date.now()+(expired?-1000:300000)).toISOString(),serverNow:new Date().toISOString()},action:'/Demo/SaveExam/ALG-X04',classList:{toggle(){}},
  querySelector:s=>controls[s.slice(6,-1).replace('question-','')],
  querySelectorAll:s=>s==='[data-exam-question]'?cards:s==='[data-question-jump]'?[jump]:radios,
  addEventListener:(name,fn)=>events[name]=fn};
 form.querySelector=s=>s==='[data-question-prev]'?controls.prev:s==='[data-question-next]'?controls.next:s==='[data-question-number]'?controls.number:controls[s.replace('[data-exam-','').replace(']','')];
 const ctx={document:{querySelector:()=>form,addEventListener(){}},window:{addEventListener(){}},location:{pathname:'/Demo/Exam/ALG-X04',assign:p=>events.redirect=p},Date,Math,Error,setInterval:fn=>intervals.push(fn),
  FormData:class{constructor(){this.values=new Map(radios.filter(r=>!r.disabled).map(r=>['answers[Q1]',r.value]));}set(k,v){this.values.set(k,v)}},
  fetch:async(url,args)=>{requests.push(args.body.values);if(fail)throw Error('offline');return{ok:true,json:async()=>({serverNowUtc:new Date().toISOString(),submitted:args.body.values.get('intent')==='submit'})}}};
 vm.runInNewContext(code,ctx);return{events,requests,radios,controls};
}
(async()=>{
 let p=page(true);await flush();assert.equal(p.requests[0].get('answers[Q1]'),'choice-1','Timeout discarded checked answer');assert.equal(p.requests[0].get('intent'),'submit');assert.equal(p.events.redirect,'/Demo/Exam/ALG-X04');
 p=page();p.events.change({target:{matches:()=>true}});await flush();assert.equal(p.requests[0].get('intent'),'save');assert.equal(p.radios[0].disabled,false);
 p=page(false,true);p.events.change({target:{matches:()=>true}});await flush();assert.equal(p.radios[0].disabled,false,'Failed save blocked changes');
 console.log('EXAM_JS_PASS: deadline submission retains choices, autosave, editable after failure');
})().catch(e=>{console.error(e);process.exitCode=1});
