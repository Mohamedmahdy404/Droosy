const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const script = fs.readFileSync('Droosy_PL/wwwroot/js/site.js', 'utf8');
function fixture(seen = false, storageFails = false) {
    const callbacks = {};
    const timers = [];
    const input = {};
    const status = {value:'Draft', addEventListener:(name, fn)=>callbacks.change=fn};
    const schedule = {hidden:false, querySelector:()=>input};
    const hint = {hidden:false, contains:()=>false, addEventListener:()=>{}};
    const close = {parentElement:hint, addEventListener:(name,fn)=>callbacks.close=fn};
    const help = {addEventListener:(name,fn)=>callbacks.help=fn};
    const submit = {disabled:false, setAttribute:(key,value)=>submit[key]=value, removeAttribute:key=>delete submit[key]};
    const form = {checkValidity:()=>true, querySelector:()=>submit, addEventListener:(name,fn)=>callbacks.submit=fn};
    const document = {activeElement:null,
        querySelectorAll:selector=>selector==='[data-hint]'?[hint]:selector==='[data-dismiss]'?[close]:selector==='form'?[form]:[submit],
        querySelector:selector=>selector==='[data-show-hints]'?help:selector==='#publication-status'?status:selector==='#schedule-field'?schedule:null};
    const context = {document, location:{pathname:'/Demo/Edit/ALG-L04',search:''},
        sessionStorage:{getItem:()=>{if(storageFails)throw Error('blocked');return seen?'seen':null},setItem:()=>{}},
        setTimeout:(fn,ms)=>{timers.push({fn,ms});return timers.length},clearTimeout:()=>{},
        window:{addEventListener:(name,fn)=>callbacks[name]=fn}};
    vm.runInNewContext(script,context);
    return {callbacks,timers,hint,status,schedule,input,submit,document};
}
let page=fixture();
assert.equal(page.hint.hidden,false);
assert.equal(page.timers[0].ms,11000);
page.timers[0].fn();
assert.equal(page.hint.hidden,true);
page.callbacks.help();
assert.equal(page.hint.hidden,false);
page.callbacks.close();
assert.equal(page.hint.hidden,true);
assert.equal(fixture(true).hint.hidden,true);
assert.equal(fixture(false,true).hint.hidden,false);
assert.equal(page.input.disabled,true);
assert.equal(page.input.required,false);
page.status.value='Scheduled'; page.callbacks.change();
assert.equal(page.schedule.hidden,false);
assert.equal(page.input.required,true);
assert.equal(page.input.disabled,false);
page.callbacks.submit();
assert.equal(page.submit.disabled,true);
page.callbacks.pageshow();
assert.equal(page.submit.disabled,false);
assert.equal(page.submit['aria-busy'],undefined);
console.log('PASS: hints first visit, timeout, close, reopen, storage fallback, scheduled field, mobile back navigation');
const named = {name:'intent',value:'approve',disabled:false,setAttribute:(key,value)=>named[key]=value,getAttribute:key=>named[key]};
page.callbacks.submit({submitter:named,preventDefault:()=>{throw Error('first submit blocked')}});
assert.equal(named.disabled,false,'Named submitter was disabled and would lose its posted intent');
assert.equal(named.value,'approve');
let prevented=false;page.callbacks.submit({submitter:named,preventDefault:()=>prevented=true});
assert.equal(prevented,true,'Repeated approval was not blocked');
console.log('PASS: named attendance approval submitter preserved; repeat submission blocked');
