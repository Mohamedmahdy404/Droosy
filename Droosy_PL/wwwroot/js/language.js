(() => {
 'use strict';
 const dictionary=window.DROOSY_EN||{},normalize=value=>value.replace(/\s+/g,' ').trim();
 const escape=value=>value.replace(/[.*+?^${}()|[\]\\]/g,'\\$&');
 const phrases=new RegExp('(?<![\\p{L}\\p{M}])(?:'+Object.keys(dictionary).sort((a,b)=>b.length-a.length).map(escape).join('|')+')(?![\\p{L}\\p{M}])','gu');
 const records=new WeakMap(),attributes=new WeakMap();
 let language=document.documentElement.lang==='en'?'en':'ar',observer;
 function english(value){const key=normalize(value);const translated=dictionary[key]??value.replace(phrases,match=>dictionary[match]);return translated.replace(/[٠-٩]/g,digit=>'٠١٢٣٤٥٦٧٨٩'.indexOf(digit)).replace(/٪/g,'%');}
 function skip(element){return !element||!!element.closest('script,style,textarea,input,[contenteditable], [translate="no"], [data-no-translate]');}
 function text(node){if(skip(node.parentElement))return;let state=records.get(node);if(!state||node.nodeValue!==state.rendered)state={original:node.nodeValue,rendered:node.nodeValue};const exact=node.parentElement.closest('[data-translate-exact]');const value=language==='en'?(exact?dictionary[normalize(state.original)]??state.original:english(state.original)):state.original;if(node.nodeValue!==value)node.nodeValue=value;state.rendered=value;records.set(node,state);}
 function translate(root){
  if(root.nodeType===3){text(root);return;}
  const walker=document.createTreeWalker(root,NodeFilter.SHOW_TEXT);while(walker.nextNode())text(walker.currentNode);
  const elements=root.querySelectorAll?root.querySelectorAll('[placeholder],[aria-label],[title],[data-localized-href]'):[];
  for(const element of elements){if(element.closest('[translate="no"],[data-no-translate]'))continue;let saved=attributes.get(element)||{};
   for(const name of ['placeholder','aria-label','title']){const value=element.getAttribute(name);if(value===null)continue;let state=saved[name];if(!state||value!==state.rendered)state={original:value};const result=language==='en'?english(state.original):state.original;if(value!==result)element.setAttribute(name,result);state.rendered=result;saved[name]=state;}attributes.set(element,saved);
  }
 }
 function apply(){
  observer?.disconnect();document.documentElement.lang=language;document.documentElement.dir=language==='en'?'ltr':'rtl';
  translate(document.documentElement);
  document.querySelectorAll('[data-language-option]').forEach(button=>{const active=button.dataset.languageOption===language;button.setAttribute('aria-pressed',String(active));button.classList.toggle('selected',active);});
  document.querySelectorAll('[data-localized-href]').forEach(link=>{link.href=link.dataset.localizedHref+(link.dataset.localizedHref.includes('?')?'&':'?')+'lang='+language;});
  document.querySelectorAll('[data-caption-language]').forEach(track=>{if(track.track)track.track.mode=track.dataset.captionLanguage===language?'showing':'disabled';});
  observer?.observe(document.documentElement,{subtree:true,childList:true,characterData:true,attributes:true,attributeFilter:['placeholder','aria-label','title']});
 }
 window.DroosyI18n={t:value=>language==='en'?english(value):value,get language(){return language;},setLanguage:value=>{language=value==='en'?'en':'ar';document.cookie='droosy-language='+language+'; Path=/; Max-Age=31536000; SameSite=Lax';apply();document.dispatchEvent(new CustomEvent('droosy-language-change'));},english};
 document.querySelectorAll('[data-language-option]').forEach(button=>button.addEventListener('click',()=>window.DroosyI18n.setLanguage(button.dataset.languageOption)));
 observer=new MutationObserver(()=>apply());apply();
})();
