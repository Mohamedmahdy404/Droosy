const fs=require('node:fs'),path=require('node:path');
const items=new Set();const arabic=/[\u0600-\u06ff]/;
function walk(dir){for(const x of fs.readdirSync(dir,{withFileTypes:true})){if(['bin','obj','Migrations'].includes(x.name))continue;const p=path.join(dir,x.name);if(x.isDirectory())walk(p);else if(/\.(cshtml|cs|js)$/.test(p)){const s=fs.readFileSync(p,'utf8');for(const m of s.matchAll(/"((?:\\.|[^"\\\r\n])*)"|'((?:\\.|[^'\\\r\n])*)'/g)){const t=m[1]??m[2];if(arabic.test(t)&&!/[<>]/.test(t))items.add(t.trim());}if(p.endsWith('.cshtml'))for(const m of s.matchAll(/>([^<>@{}\r\n]*[\u0600-\u06ff][^<>@{}\r\n]*)(?=<|@)/g))items.add(m[1].trim());}}}
for(const dir of ['Droosy_PL/Views','Droosy_PL/Controllers','Droosy_PL/Models','Droosy_PL/Services','Droosy_PL/wwwroot/js','Droosy_BL','Droosy_DAL/Seeding'])walk(dir);
fs.writeFileSync('.verification/i18n-source.json',JSON.stringify([...items].filter(Boolean),null,2));console.log(items.size+' source phrases');
