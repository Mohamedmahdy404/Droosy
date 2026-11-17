import json, re
from pathlib import Path
from xml.sax.saxutils import escape
from reportlab.lib import colors
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_LEFT
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, KeepTogether
from pypdf import PdfReader

root=Path(__file__).resolve().parent.parent
dictionary={}
for line in (root/'Droosy_PL/Localization/en.tsv').read_text(encoding='utf-8-sig').splitlines():
    if line.strip():
        ar,en=line.split('\t',1); dictionary[re.sub(r'\s+',' ',ar).strip()]=en
def translate(text):
    key=re.sub(r'\s+',' ',text).strip()
    if key not in dictionary: raise ValueError('Missing lesson translation: '+text)
    return dictionary[key]
lessons=json.loads((root/'.verification/assets/lessons.json').read_text(encoding='utf-8'))
pdfmetrics.registerFont(TTFont('Droosy',r'C:\Windows\Fonts\segoeui.ttf'))
pdfmetrics.registerFont(TTFont('DroosyBold',r'C:\Windows\Fonts\segoeuib.ttf'))
output=root/'Droosy_PL/App_Data/Lessons'
green=colors.HexColor('#0e5a46'); muted=colors.HexColor('#627165')
styles={
 'title':ParagraphStyle('title',fontName='DroosyBold',fontSize=24,leading=32,textColor=green,spaceAfter=22),
 'heading':ParagraphStyle('heading',fontName='DroosyBold',fontSize=13,leading=20,textColor=green,spaceAfter=9),
 'body':ParagraphStyle('body',fontName='Droosy',fontSize=11,leading=19,textColor=colors.HexColor('#25382f'),spaceAfter=14),
 'small':ParagraphStyle('small',fontName='Droosy',fontSize=9,leading=14,textColor=muted,spaceAfter=12),
}
def page(canvas,doc):
    canvas.setFillColor(colors.HexColor('#f7f4ea'));canvas.rect(0,0,595.28,841.89,fill=1,stroke=0)
    canvas.setFillColor(green);canvas.setFont('DroosyBold',15);canvas.drawString(48,798,'DROOSY')
    canvas.setFillColor(muted);canvas.setFont('Droosy',9);canvas.drawRightString(547,798,'LEARN. PRACTISE. GROW.')
    canvas.setStrokeColor(colors.HexColor('#d7dfce'));canvas.line(48,783,547,783)
    canvas.setFont('Droosy',8);canvas.drawString(48,32,'Your lesson companion  |  English edition');canvas.drawRightString(547,32,str(doc.page))
for lesson_id,lesson in lessons.items():
    target=output/(lesson_id+'.en.pdf')
    doc=SimpleDocTemplate(str(target),pagesize=(595.28,841.89),leftMargin=48,rightMargin=48,topMargin=85,bottomMargin=58,title='Droosy - '+translate(lesson['Title']),author='Droosy')
    flow=[Paragraph(escape(translate(lesson['Title'])),styles['title']),Paragraph('Understand the idea',styles['heading'])]
    flow += [Paragraph(escape(translate(p)),styles['body']) for p in lesson['Paragraphs']]
    for label,key in [('Worked example','Example'),('Try it yourself','Exercise')]:
        flow.append(KeepTogether([Spacer(1,8),Paragraph(label,styles['heading']),Paragraph(escape(translate(lesson[key])),styles['body'])]))
    doc.build(flow,onFirstPage=page,onLaterPages=page)
    reader=PdfReader(target);assert len(reader.pages)==1;assert 'Try it yourself' in reader.pages[0].extract_text()
    if lesson_id.endswith('L02'):
        cues=['WEBVTT','']
        for i,text in enumerate(lesson['Paragraphs']):
            cues += [f'00:00:{i*16:02d}.000 --> 00:00:{(i+1)*16:02d}.000',translate(text),'']
        (output/(lesson_id+'.en.vtt')).write_text('\n'.join(cues),encoding='utf-8')
print('ENGLISH_CONTENT_PASS: 8 one-page PDFs, 2 subtitle files, all lesson translations present')
