Add-Type -AssemblyName System.Drawing
$assetDir=Join-Path $PSScriptRoot 'assets'
$outputDir=Join-Path (Split-Path $PSScriptRoot) 'Droosy_PL\App_Data\Lessons'
New-Item -ItemType Directory -Path $assetDir,$outputDir -Force | Out-Null
$source=Get-Content -Raw -Encoding UTF8 (Join-Path (Split-Path $PSScriptRoot) 'Droosy_DAL\Seeding\DemoLessonContent.cs')
$pattern='\["(?<id>[A-Z]+-L\d+)"\] = new\("(?<title>[^"]+)", \[(?<paragraphs>.*?)\], "(?<example>[^"]+)", "(?<exercise>[^"]+)"\)'
$green=[Drawing.ColorTranslator]::FromHtml('#0E5A46'); $ivory=[Drawing.ColorTranslator]::FromHtml('#F7F4EA'); $sage=[Drawing.ColorTranslator]::FromHtml('#E8EFE2'); $ink=[Drawing.ColorTranslator]::FromHtml('#20372F'); $muted=[Drawing.ColorTranslator]::FromHtml('#64766B'); $gold=[Drawing.ColorTranslator]::FromHtml('#AD883F')
$rtl=[Drawing.StringFormat]::new();$rtl.FormatFlags=[Drawing.StringFormatFlags]::DirectionRightToLeft;$rtl.Alignment=[Drawing.StringAlignment]::Near
function Text($g,$text,$x,$y,$width,$height,$size,$color,$bold=$false) {
    $font=[Drawing.Font]::new('Tahoma',$size, $(if($bold){[Drawing.FontStyle]::Bold}else{[Drawing.FontStyle]::Regular}),[Drawing.GraphicsUnit]::Pixel)
    $brush=[Drawing.SolidBrush]::new($color)
    $g.DrawString($text,$font,$brush,[Drawing.RectangleF]::new($x,$y,$width,$height),$rtl)
    $font.Dispose();$brush.Dispose()
}
$manifest=@()
foreach($m in [regex]::Matches($source,$pattern)) {
    $id=$m.Groups['id'].Value;$title=$m.Groups['title'].Value;$paragraphs=@([regex]::Matches($m.Groups['paragraphs'].Value,'"([^"]+)"') | ForEach-Object {$_.Groups[1].Value});$example=$m.Groups['example'].Value;$exercise=$m.Groups['exercise'].Value
    $page=[Drawing.Bitmap]::new(1240,1754,[Drawing.Imaging.PixelFormat]::Format32bppArgb);$g=[Drawing.Graphics]::FromImage($page);$g.Clear($ivory);$g.TextRenderingHint=[Drawing.Text.TextRenderingHint]::AntiAliasGridFit
    $brush=[Drawing.SolidBrush]::new($green);$g.FillRectangle($brush,0,0,1240,250);$brush.Dispose()
    Text $g 'دروسي  /  ملزمة الدرس' 80 52 1080 50 29 $sage
    Text $g $title 80 120 1080 92 48 ([Drawing.Color]::White) $true
    Text $g 'نفهم الفكرة' 85 294 1070 55 35 $green $true
    $y=375
    foreach($p in $paragraphs){Text $g $p 85 $y 1070 175 29 $ink;$y+=184}
    $brush=[Drawing.SolidBrush]::new($sage);$g.FillRectangle($brush,70,970,1100,245);$brush.Dispose()
    Text $g 'مثال محلول' 105 998 1030 50 30 $green $true
    Text $g $example 105 1072 1030 108 32 $ink
    Text $g 'جرّب بنفسك' 85 1275 1070 50 33 $green $true
    Text $g $exercise 85 1350 1070 180 29 $ink
    Text $g 'خطوة صغيرة، فهم أوضح.  |  محتوى تعليمي تجريبي' 85 1630 1070 50 24 $muted
    $page.Save((Join-Path $assetDir ($id+'.png')),[Drawing.Imaging.ImageFormat]::Png);$g.Dispose();$page.Dispose()
    $manifest+=@{id=$id;title=$title}
    if($id.EndsWith('L02')){
        $prefix=$id.Substring(0,3)
        $formulas=if($prefix -eq 'ALG'){@('٢س + ٤ = ١٤','٢س = ١٤ − ٤','٢س = ١٠  ←  س = ٥','٢ × ٥ + ٤ = ١٤')}else{@('مجموع زوايا المثلث = ١٨٠°','٥٠° + ٦٠° = ١١٠°','س = ١٨٠° − ١١٠°','س = ٧٠°')}
        $captions=if($prefix -eq 'ALG'){@('نبدأ بمعادلة من خطوتين. نريد أن نعرف قيمة س.','نطرح ٤ من الطرفين حتى نحافظ على التساوي.','نقسم الطرفين على ٢، فنحصل على قيمة المجهول.','نتحقق بالتعويض: الطرفان متساويان. جرّب تدريب الملزمة.')}else{@('أي مثلث له ثلاث زوايا داخلية مجموعها ١٨٠ درجة.','نعرف زاويتين؛ نجمع قياسيهما أولًا.','نطرح مجموع الزاويتين من ١٨٠ درجة.','الزاوية المجهولة ٧٠ درجة. جرّب تدريب الملزمة.')}
        for($i=0;$i -lt 4;$i++){
            $slide=[Drawing.Bitmap]::new(960,540,[Drawing.Imaging.PixelFormat]::Format32bppArgb);$g=[Drawing.Graphics]::FromImage($slide);$g.Clear($green);$g.TextRenderingHint=[Drawing.Text.TextRenderingHint]::AntiAliasGridFit
            Text $g ('دروسي  /  '+$title) 45 28 870 55 25 $sage
            Text $g ('الخطوة '+($i+1)+' من ٤') 45 100 870 45 24 $gold
            Text $g $formulas[$i] 45 186 870 115 52 ([Drawing.Color]::White) $true
            Text $g $captions[$i] 58 344 844 120 27 $sage
            $b=[Drawing.SolidBrush]::new($gold);$g.FillRectangle($b,45,500, [int](870*($i+1)/4),5);$b.Dispose()
            $slide.Save((Join-Path $assetDir ($prefix+'-'+$i+'.bmp')),[Drawing.Imaging.ImageFormat]::Bmp)
            if($i -eq 0){$slide.Save((Join-Path $outputDir ($id+'.png')),[Drawing.Imaging.ImageFormat]::Png)}
            $g.Dispose();$slide.Dispose()
        }
    }
}
$manifest | ConvertTo-Json | Set-Content -Encoding UTF8 (Join-Path $assetDir 'manifest.json')
Write-Output "DRAWN: $($manifest.Count) lesson pages and 8 teaching slides"
