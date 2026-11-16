from PIL import Image
import numpy as np
from pathlib import Path
assets=Path('.verification/assets')
for course in ['ALG','GEO']:
    raw=np.frombuffer((assets/(course+'-decoded.nv12')).read_bytes(),dtype=np.uint8)
    h=int(len(raw)/960*2/3)
    plane=raw.reshape(h*3//2,960)
    y=plane[:540].astype(float)-16
    uv=plane[h:h+270].astype(float)-128
    u=uv[:,0::2].repeat(2,0).repeat(2,1)
    v=uv[:,1::2].repeat(2,0).repeat(2,1)
    rgb=np.stack([1.164*y+1.596*v,1.164*y-.392*u-.813*v,1.164*y+2.017*u],axis=-1).clip(0,255).astype(np.uint8)
    image=Image.fromarray(rgb);image.save(assets/(course+'-native-decoded.png'))
    source=np.asarray(Image.open(assets/(course+'-0.bmp')).convert('RGB')).astype(float)
    error=np.abs(source-rgb).mean();assert error<3,(course,error)
    print('NATIVE_FRAME_ORIENTATION_VERIFIED:',course,round(error,3))
