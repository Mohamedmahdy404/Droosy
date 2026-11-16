Add-Type -AssemblyName System.Drawing
$directory=Join-Path (Split-Path $PSScriptRoot) 'Droosy_PL\App_Data\SeedAttachments';New-Item -ItemType Directory -Path $directory -Force | Out-Null
$bitmap=[Drawing.Bitmap]::new(1000,760);$g=[Drawing.Graphics]::FromImage($bitmap);$g.SmoothingMode=[Drawing.Drawing2D.SmoothingMode]::AntiAlias;$g.TextRenderingHint=[Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$g.Clear([Drawing.ColorTranslator]::FromHtml('#F7F4EA'));$green=[Drawing.ColorTranslator]::FromHtml('#0E5A46');$brush=[Drawing.SolidBrush]::new($green);$font=[Drawing.Font]::new('Tahoma',31,[Drawing.FontStyle]::Bold,[Drawing.GraphicsUnit]::Pixel)
$rtl=[Drawing.StringFormat]::new();$rtl.FormatFlags=[Drawing.StringFormatFlags]::DirectionRightToLeft
$g.DrawString('دروسي | مراجعة مساحة المربع',$font,$brush,[Drawing.RectangleF]::new(50,35,900,65),$rtl)
$fill=[Drawing.SolidBrush]::new([Drawing.ColorTranslator]::FromHtml('#E8EFE2'));$g.FillRectangle($fill,340,150,320,320);$pen=[Drawing.Pen]::new($green,5);$g.DrawRectangle($pen,340,150,320,320);$g.DrawString('4',$font,$brush,485,105);$g.DrawString('4',$font,$brush,680,290)
$g.DrawString('المساحة = طول الضلع × نفسه',$font,$brush,[Drawing.RectangleF]::new(50,535,900,65),$rtl)
$math=[Drawing.Font]::new('Tahoma',44,[Drawing.FontStyle]::Regular,[Drawing.GraphicsUnit]::Pixel);$g.DrawString('4 × 4 = 16',$math,$brush,380,625)
$bitmap.Save((Join-Path $directory 'geometry-solution.png'),[Drawing.Imaging.ImageFormat]::Png);$g.Dispose();$bitmap.Dispose();$pen.Dispose();$brush.Dispose();$fill.Dispose();$font.Dispose();$math.Dispose();$rtl.Dispose()
