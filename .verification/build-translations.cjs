const fs=require('node:fs');const map={};
for(const line of fs.readFileSync('Droosy_PL/Localization/en.tsv','utf8').replace(/^\uFEFF/,'').split(/\r?\n/)){
 if(!line.trim())continue;const separator=line.indexOf('\t');if(separator<1)throw Error('Missing tab: '+line);
 const ar=line.slice(0,separator).replace(/\s+/g,' ').trim(),en=line.slice(separator+1).trim();
 if(map[ar]&&map[ar]!==en)throw Error('Conflicting translation: '+ar);map[ar]=en;
}
fs.writeFileSync('Droosy_PL/wwwroot/js/translations.en.js','// Generated from Localization/en.tsv by .verification/build-translations.cjs\nwindow.DROOSY_EN='+JSON.stringify(map,null,2)+';\n');
console.log('TRANSLATIONS_BUILT: '+Object.keys(map).length);
