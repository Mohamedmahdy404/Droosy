// Encodes locally drawn teaching slides using the Windows Media Foundation sink writer.
// API reference: https://learn.microsoft.com/windows/win32/medfound/tutorial--using-the-sink-writer-to-encode-video
using System.Runtime.InteropServices;

unsafe class Program
{
    [DllImport("ole32.dll")] static extern int CoInitializeEx(nint reserved, uint flags);
    [DllImport("mfplat.dll")] static extern int MFStartup(uint version, uint flags);
    [DllImport("mfplat.dll")] static extern int MFShutdown();
    [DllImport("mfplat.dll")] static extern int MFCreateMediaType(out nint type);
    [DllImport("mfplat.dll")] static extern int MFCreateAttributes(out nint attributes, uint size);
    [DllImport("mfplat.dll")] static extern int MFCreateSample(out nint sample);
    [DllImport("mfplat.dll")] static extern int MFCreateMemoryBuffer(uint size, out nint buffer);
    [DllImport("mfreadwrite.dll", CharSet=CharSet.Unicode)] static extern int MFCreateSinkWriterFromURL(string url, nint stream, nint attributes, out nint writer);
    static void Check(int hr) { if(hr < 0) Marshal.ThrowExceptionForHR(hr); }
    static nint Fn(nint obj, int slot) => ((nint*)(*(nint*)obj))[slot];
    static void Release(nint obj) { if(obj != 0) ((delegate* unmanaged[Stdcall]<nint,uint>)Fn(obj,2))(obj); }
    static void U32(nint obj, string key, uint value) { var id=new Guid(key); Check(((delegate* unmanaged[Stdcall]<nint,Guid*,uint,int>)Fn(obj,21))(obj,&id,value)); }
    static void U64(nint obj, string key, ulong value) { var id=new Guid(key); Check(((delegate* unmanaged[Stdcall]<nint,Guid*,ulong,int>)Fn(obj,22))(obj,&id,value)); }
    static void GuidValue(nint obj,string key,string value) { var id=new Guid(key);var val=new Guid(value);Check(((delegate* unmanaged[Stdcall]<nint,Guid*,Guid*,int>)Fn(obj,24))(obj,&id,&val)); }
    static void Format(nint type, bool encoded)
    {
        GuidValue(type,"48eba18e-f8c9-4687-bf11-0a74c9f96a8f","73646976-0000-0010-8000-00aa00389b71");
        GuidValue(type,"f7e34c9a-42e8-4714-b74b-cb29d72c35e5",encoded?"34363248-0000-0010-8000-00aa00389b71":"00000016-0000-0010-8000-00aa00389b71");
        U32(type,"e2724bb8-e676-4806-b4b2-a8d6efb44ccd",2);
        U64(type,"1652c33d-d6b2-4012-b834-72030849a37d",((ulong)960<<32)|540);
        U64(type,"c459a2e8-3d2c-4e44-b132-fee5156c7bb0",((ulong)10<<32)|1);
        U64(type,"c6376a1e-8d0a-4027-be45-6d9a0ad39bb6",((ulong)1<<32)|1);
        if(encoded) U32(type,"20332624-fb0d-4d9e-bd0d-cbf6786c102e",700000);
    }
    [DllImport("mfreadwrite.dll", CharSet=CharSet.Unicode)] static extern int MFCreateSourceReaderFromURL(string url,nint attributes,out nint reader);
    static void Decode(string path, string rawOutput, bool nv12 = false)
    {
        Check(CoInitializeEx(0,0)); Check(MFStartup(0x20070,0));
        nint reader=0,type=0,sample=0,buffer=0,attributes=0;
        try
        {
            Check(MFCreateAttributes(out attributes,1));
            U32(attributes,"fb394f3d-ccf1-42ee-bbb3-f9b845d5681d",1);
            Check(MFCreateSourceReaderFromURL(Path.GetFullPath(path),attributes,out reader));
            Check(MFCreateMediaType(out type));
            GuidValue(type,"48eba18e-f8c9-4687-bf11-0a74c9f96a8f","73646976-0000-0010-8000-00aa00389b71");
            GuidValue(type,"f7e34c9a-42e8-4714-b74b-cb29d72c35e5",nv12?"3231564e-0000-0010-8000-00aa00389b71":"00000016-0000-0010-8000-00aa00389b71");
            Check(((delegate* unmanaged[Stdcall]<nint,uint,nint,nint,int>)Fn(reader,7))(reader,0xfffffffc,0,type));
            for(int i=0;i<10 && sample==0;i++)
            {
                uint stream,flags;long timestamp;
                Check(((delegate* unmanaged[Stdcall]<nint,uint,uint,uint*,uint*,long*,nint*,int>)Fn(reader,9))(reader,0xfffffffc,0,&stream,&flags,&timestamp,&sample));
            }
            if(sample==0)throw new Exception("No decoded video frame");
            Check(((delegate* unmanaged[Stdcall]<nint,nint*,int>)Fn(sample,41))(sample,&buffer));
            byte* pixels;uint maximum,current;
            Check(((delegate* unmanaged[Stdcall]<nint,byte**,uint*,uint*,int>)Fn(buffer,3))(buffer,&pixels,&maximum,&current));
            try {File.WriteAllBytes(rawOutput,new ReadOnlySpan<byte>(pixels,(int)current).ToArray());}
            finally {Check(((delegate* unmanaged[Stdcall]<nint,int>)Fn(buffer,4))(buffer));}
            Console.WriteLine("DECODED_FRAME: "+path+"; "+current+" bytes; "+(nv12?"NV12":"RGB32"));
        }
        finally {Release(buffer);Release(sample);Release(type);Release(reader);Release(attributes);MFShutdown();}
    }
    static void Main(string[] args)
    {
        if(args.Length==3 && args[0]=="--decode-yuv") { Decode(args[1],args[2],true);return; }
        if(args.Length==3 && args[0]=="--decode") { Decode(args[1],args[2]);return; }
        if(args.Length != 3) throw new ArgumentException("output.mp4 slide-directory course-prefix");
        Check(CoInitializeEx(0,0)); Check(MFStartup(0x20070,0));
        nint writer=0,output=0,input=0,attributes=0;
        try
        {
            Check(MFCreateAttributes(out attributes,1));
            U32(attributes,"08b845d8-2b74-4afe-9d53-be16d2d5ae4f",1);
            Check(MFCreateSinkWriterFromURL(Path.GetFullPath(args[0]),0,attributes,out writer));
            Check(MFCreateMediaType(out output)); Format(output,true);
            uint index; Check(((delegate* unmanaged[Stdcall]<nint,nint,uint*,int>)Fn(writer,3))(writer,output,&index));
            Check(MFCreateMediaType(out input));Format(input,false);
            Check(((delegate* unmanaged[Stdcall]<nint,uint,nint,nint,int>)Fn(writer,4))(writer,index,input,0));
            Check(((delegate* unmanaged[Stdcall]<nint,int>)Fn(writer,5))(writer));
            var slides=Enumerable.Range(0,4).Select(i=>{
                var bmp=File.ReadAllBytes(Path.Combine(args[1],args[2]+"-"+i+".bmp"));
                var offset=BitConverter.ToInt32(bmp,10); return bmp[offset..];
            }).ToArray();
            const long frameDuration=1_000_000;
            for(int frame=0;frame<480;frame++)
            {
                nint buffer=0,sample=0;
                try
                {
                    var pixels=slides[frame/120];
                    Check(MFCreateMemoryBuffer((uint)pixels.Length,out buffer));
                    byte* target; uint max,current;
                    Check(((delegate* unmanaged[Stdcall]<nint,byte**,uint*,uint*,int>)Fn(buffer,3))(buffer,&target,&max,&current));
                    fixed(byte* source=pixels) Buffer.MemoryCopy(source,target,max,pixels.Length);
                    Check(((delegate* unmanaged[Stdcall]<nint,int>)Fn(buffer,4))(buffer));
                    Check(((delegate* unmanaged[Stdcall]<nint,uint,int>)Fn(buffer,6))(buffer,(uint)pixels.Length));
                    Check(MFCreateSample(out sample));
                    Check(((delegate* unmanaged[Stdcall]<nint,nint,int>)Fn(sample,42))(sample,buffer));
                    Check(((delegate* unmanaged[Stdcall]<nint,long,int>)Fn(sample,36))(sample,frame*frameDuration));
                    Check(((delegate* unmanaged[Stdcall]<nint,long,int>)Fn(sample,38))(sample,frameDuration));
                    Check(((delegate* unmanaged[Stdcall]<nint,uint,nint,int>)Fn(writer,6))(writer,index,sample));
                }
                finally { Release(sample);Release(buffer); }
            }
            Check(((delegate* unmanaged[Stdcall]<nint,int>)Fn(writer,11))(writer));
            Console.WriteLine("ENCODED: "+args[0]+"; 960x540 H.264; 480 frames / 10 fps = 48 seconds");
        }
        finally { Release(writer);Release(output);Release(input);Release(attributes);MFShutdown(); }
    }
}
