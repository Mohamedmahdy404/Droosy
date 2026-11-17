const fs=require('node:fs'),vm=require('node:vm'),assert=require('node:assert/strict');
const listeners={},buttons=['ar','en'].map(language=>({dataset:{languageOption:language},setAttribute(k,v){this[k]=v},classList:{toggle(){}},addEventListener(name,fn){listeners[language]=fn}}));
const nodes=[{nodeType:3,nodeValue:'الواجبات',parentElement:{closest:()=>null}},{nodeType:3,nodeValue:'  مجموع زوايا المثلث  ',parentElement:{closest:()=>null}}];
const element={lang:'ar',dir:'rtl',querySelectorAll:()=>[]};let walker;
const document={documentElement:element,cookie:'',createTreeWalker:()=>{let i=-1;return{nextNode(){i++;this.currentNode=nodes[i];return i<nodes.length}}},querySelectorAll:s=>s==='[data-language-option]'?buttons:[],dispatchEvent(){}};
const context={window:{},document,NodeFilter:{SHOW_TEXT:4},CustomEvent:class{},MutationObserver:class{observe(){}disconnect(){}}};
vm.runInNewContext(fs.readFileSync('Droosy_PL/wwwroot/js/translations.en.js','utf8'),context);
vm.runInNewContext(fs.readFileSync('Droosy_PL/wwwroot/js/language.js','utf8'),context);
assert.equal(element.lang,'ar');assert.equal(nodes[0].nodeValue,'الواجبات');
listeners.en();assert.equal(element.lang,'en');assert.equal(element.dir,'ltr');assert.equal(nodes[0].nodeValue,'Assignments');assert.ok(document.cookie.includes('droosy-language=en'));assert.equal(buttons[1]['aria-pressed'],'true');
listeners.ar();assert.equal(element.dir,'rtl');assert.equal(nodes[0].nodeValue,'الواجبات');
assert.equal(context.window.DroosyI18n.english('أساسيات الجبر'),'Algebra fundamentals');
assert.equal(context.window.DroosyI18n.english('اختبار المعادلات ٤'),'Algebra exam 4');
assert.ok(!/[\u0600-\u06ff]/.test(context.window.DroosyI18n.english('٥ أسئلة · ٥ دقائق · ١٠ درجات')));
if(fs.existsSync('.verification/rendered-text.json')){
 const texts=JSON.parse(fs.readFileSync('.verification/rendered-text.json','utf8'));const missing=texts.map(text=>({text,result:context.window.DroosyI18n.english(text)})).filter(x=>/[\u0600-\u06ff]/.test(x.result)&&x.result.trim().length>2);
 fs.writeFileSync('.verification/missing-translations.json',JSON.stringify(missing,null,2));console.log('UNTRANSLATED_RENDERED_TEXTS: '+missing.length);
}
console.log('LANGUAGE_PASS: Arabic default, English direction/content, switch back, persisted choice, button state, demo titles and numbers');
