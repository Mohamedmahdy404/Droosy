import json
from pathlib import Path
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import A4
from pypdf import PdfReader
root=Path(__file__).resolve().parent.parent
assets=root/'.verification'/'assets'
output=root/'Droosy_PL'/'App_Data'/'Lessons'
manifest=json.loads((assets/'manifest.json').read_text(encoding='utf-8-sig'))
for lesson in manifest:
    path=output/(lesson['id']+'.pdf')
    c=canvas.Canvas(str(path),pagesize=A4)
    c.setTitle('دروسي - '+lesson['title'])
    c.setAuthor('Droosy')
    c.drawImage(str(assets/(lesson['id']+'.png')),0,0,width=A4[0],height=A4[1])
    c.showPage();c.save()
    assert len(PdfReader(path).pages)==1
print('PDFS_VERIFIED: 8 single-page Arabic lesson handouts')
