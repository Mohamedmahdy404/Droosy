using Droosy_DAL.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace Droosy_PL.TagHelpers;
[HtmlTargetElement("form")]
public class DemoGenerationTagHelper(DemoScope scope):TagHelper
{
 public override void Process(TagHelperContext context,TagHelperOutput output)
 {
  if(output.Attributes["method"]?.Value?.ToString()?.Equals("post",StringComparison.OrdinalIgnoreCase)==true)
   output.PostContent.AppendHtml($"<input type=\"hidden\" name=\"demoGeneration\" value=\"{scope.GenerationId:D}\" />");
 }
}
