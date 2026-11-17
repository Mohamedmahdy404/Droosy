import json,re
from pathlib import Path
from html.parser import HTMLParser
class Texts(HTMLParser):
    def __init__(self):super().__init__();self.skip=0;self.rows=[]
    def handle_starttag(self,tag,attrs):
        if tag in ['script','style']:self.skip+=1
        for k,v in attrs:
            if k in ['aria-label','placeholder','title'] and v:self.rows.append(v)
    def handle_endtag(self,tag):
        if tag in ['script','style']:self.skip=max(0,self.skip-1)
    def handle_data(self,data):
        if not self.skip and re.search(r'[\u0600-\u06ff]',data):self.rows.append(data.strip())
rows=set()
for file in Path('.verification/translation-html').glob('*.html'):
    parser=Texts();parser.feed(file.read_text(encoding='utf-8'));rows.update(parser.rows)
Path('.verification/rendered-text.json').write_text(json.dumps(sorted(rows)),encoding='utf-8')
print('RENDERED_TEXTS: '+str(len(rows)))
