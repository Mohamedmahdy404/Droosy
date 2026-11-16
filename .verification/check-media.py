from PIL import Image, ImageOps
from pathlib import Path
import numpy as np, struct, json
root=Path(__file__).resolve().parent.parent
assets=root/'.verification'/'assets'
results=[]
for course in ['ALG','GEO']:
    source=Image.open(assets/(course+'-0.bmp')).convert('RGB')
    pixels=(assets/(course+'-decoded.bgra')).read_bytes()
    decoded=Image.frombytes('RGB',(960,540),pixels,'raw','BGRX',0,1)
    error=np.mean(np.abs(np.asarray(source).astype(float)-np.asarray(decoded).astype(float)))
    assert error<3,(course,error)
    decoded.save(assets/(course+'-decoded.png'))
    data=(root/'Droosy_PL'/'App_Data'/'Lessons'/(course+'-L02.mp4')).read_bytes()
    def boxes(buf,start,end):
        while start+8<=end:
            length,kind=struct.unpack_from('>I4s',buf,start)
            if not length: length=end-start
            assert length>=8
            yield kind,start+8,start+length
            start+=length
    duration=None
    for kind,a,b in boxes(data,0,len(data)):
        if kind==b'moov':
            for kind2,c,d in boxes(data,a,b):
                if kind2==b'mvhd':
                    version=data[c]
                    offset=c+20 if version else c+12
                    scale=struct.unpack_from('>I',data,offset)[0]
                    dur=struct.unpack_from('>Q' if version else '>I',data,offset+4)[0]
                    duration=dur/scale
    assert duration==48,(course,duration)
    assert b'avc1' in data
    results.append({'course':course,'durationSeconds':duration,'codec':'H.264/AVC','decodedFirstFrameMeanError':round(error,2),'sizeBytes':len(data)})
(assets/'media-checks.json').write_text(json.dumps(results,indent=2))
print('MEDIA_CHECKS_PASSED:',json.dumps(results))
