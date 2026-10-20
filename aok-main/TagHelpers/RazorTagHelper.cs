using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AOK.TagHelpers
{
    [HtmlTargetElement("li", Attributes = "is-active-route")]
    public class ActiveRouteTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("active-class")]
        public string ActiveClass { get; set; } = "active";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString();

            if (string.Equals(Controller, currentController, StringComparison.OrdinalIgnoreCase))
            {
                var existingClass = output.Attributes["class"]?.Value?.ToString() ?? "";
                if (!existingClass.Contains(ActiveClass))
                {
                    output.Attributes.SetAttribute("class", $"{existingClass} {ActiveClass}".Trim());
                }
            }

            output.Attributes.RemoveAll("is-active-route");
        }
    }
}
