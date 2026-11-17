const fs=require('node:fs'),vm=require('node:vm'),assert=require('node:assert/strict');
const events={},label={textContent:''},input={dataset:{remainingBytes:String(10*1024*1024)},files:[],setCustomValidity(value){this.error=value},reportValidity(){this.reported=true},addEventListener(name,fn){events.change=fn}};
const document={querySelector:s=>s==='[data-message-file]'?input:label,addEventListener:(name,fn)=>events[name]=fn};
vm.runInNewContext(fs.readFileSync('Droosy_PL/wwwroot/js/message-compose.js','utf8'),{document,window:{}});
input.files=[{name:'exact.png',size:2*1024*1024}];events.change();assert.equal(input.error,'');
input.files=[{name:'too-big.png',size:2*1024*1024+1}];events.change();assert.ok(input.error.includes('2 MB'));
input.dataset.remainingBytes='10';input.files=[{name:'full.png',size:11}];events.change();assert.ok(input.error.includes('10 MB'));
input.files=[];events.change();assert.equal(input.error,'');assert.equal(label.textContent,'');
console.log('MESSAGE_UI_PASS: exact 2 MB accepted, oversize rejected, remaining shared quota checked, text-only allowed');
