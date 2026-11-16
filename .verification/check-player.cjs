const fs=require('node:fs'), vm=require('node:vm'), assert=require('node:assert/strict');
const code=fs.readFileSync('Droosy_PL/wwwroot/js/lesson-player.js','utf8');
function page(options={}){
    const events={}, calls=[], beacons=[], requests=[]; const ranges=[[0,16.8]];
    const ui=Object.fromEntries(['state','message','save-state','meter','bar','retry'].map(k=>[k,{textContent:'',hidden:true,style:{},setAttribute:(name,value)=>{},addEventListener:(name,fn)=>events[k+'-'+name]=fn}]));
    const video={dataset:{position:'16.8',saveUrl:'/Demo/SaveVideoProgress/ALG-L02'},currentTime:0,duration:48,paused:false,
        played:{get length(){return ranges.length},start:i=>ranges[i][0],end:i=>ranges[i][1]},addEventListener:(name,fn)=>events[name]=fn};
    const document={visibilityState:'visible',querySelector:selector=>selector==='meta[name="droosy-generation"]'?{content:'test-generation'}:selector==='[data-lesson-video]'?(options.readOnly?null:video):selector==='[data-retry-progress]'?ui.retry:ui[selector.replace('[data-video-','').replace(']','')],addEventListener:(name,fn)=>events[name]=fn};
    const context={document,window:{addEventListener:(name,fn)=>events[name]=fn},navigator:{sendBeacon:(url,blob)=>{beacons.push({url,blob});return true}},Blob,Array,Date,Number,Math,Error,JSON,
        fetch:async(url,args)=>{calls.push(JSON.parse(args.body));requests.push(args);if(options.fail)throw Error('offline');return {ok:true,json:async()=>({completed:!!options.completed,watchedSeconds:options.completed?44:16.8})}}};
    vm.runInNewContext(code,context);return{events,calls,beacons,requests,video,ui,document,ranges};
}
const tick=()=>new Promise(resolve=>setImmediate(resolve));
(async()=>{
    let p=page();p.events.loadedmetadata();assert.equal(p.video.currentTime,16.8,'Resume position missing');
    p.events.timeupdate();await tick();assert.equal(p.calls.length,1);assert.deepEqual(p.calls[0].played,[[0,16.8]]);assert.equal(p.ui.bar.style.width,'35%');assert.ok(p.ui['save-state'].textContent.includes('تم حفظ'));
    p.video.currentTime=47;p.events.pause();await tick();assert.deepEqual(p.calls.at(-1).played,[[0,16.8]],'Seeking invented watched time');assert.equal(p.calls.at(-1).position,47);
    p.events.pagehide();await tick();assert.equal(p.requests.at(-1).keepalive,true);assert.equal(p.requests.at(-1).headers['X-Droosy-Generation'],'test-generation');assert.equal(p.calls.at(-1).position,47);
    let completed=page({completed:true});completed.events.pause();await tick();assert.equal(completed.ui.state.textContent,'مكتمل');
    let failed=page({fail:true});failed.events.pause();await tick();assert.equal(failed.ui.retry.hidden,false);assert.ok(failed.ui['save-state'].textContent.includes('تعذّر'));
    let readOnly=page({readOnly:true});assert.equal(readOnly.calls.length,0);assert.deepEqual(Object.keys(readOnly.events),[]);
    console.log('PLAYER_CHECKS_PASSED: resume, actual played ranges, seek behavior, unload save, completion, retry, read-only parent/teacher');
})().catch(e=>{console.error(e);process.exitCode=1});
