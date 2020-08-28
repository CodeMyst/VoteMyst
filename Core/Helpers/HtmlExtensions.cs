using System;
using System.Linq;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VoteMyst
{
    public static class HtmlExtensions
    {
        public static IHtmlContent ConditionalAttribute(this IHtmlHelper html, bool condition, string attributeName)
        {
            return new HtmlString(condition ? attributeName : string.Empty);
        }

        public static IHtmlContent TextNewlines(this IHtmlHelper html, string text)
        {
            return new HtmlString(text.Replace(Environment.NewLine, "<br />"));
        }
    }    
}