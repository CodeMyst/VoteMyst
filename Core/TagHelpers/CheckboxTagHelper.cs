using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VoteMyst.TagHelpers
{
    [HtmlTargetElement("checkbox")]
    public class CheckboxTagHelper : TagHelper
    {
        public bool Checked { get; set; }

        public CheckboxTagHelper()
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            
            var input = new TagBuilder("input");
            foreach (var a in output.Attributes)
                input.Attributes.Add(a.Name, a.Value.ToString());
            input.Attributes.Add("type", "checkbox");
            if (Checked)
                input.Attributes.Add("checked", "");

            output.Attributes.Clear();
            output.Attributes.Add("class", "checkbox");
            output.Content.AppendHtml(input);

            var box = new TagBuilder("div");
            output.Content.AppendHtml(box);

            await Task.CompletedTask;
        }
    }
}
