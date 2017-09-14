using System;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Janitra.TagHelpers
{
    public class ExecutionResultTagHelper : TagHelper
    {
	    public ExecutionResult Result { get; set; }

	    public override void Process(TagHelperContext context, TagHelperOutput output)
	    {
		    output.TagName = "span";
		    output.TagMode = TagMode.StartTagAndEndTag;
		    output.Content.SetContent(Result.ToString());
		    output.Attributes.SetAttribute("class", "tag is-medium " + StatusClass);
	    }

	    private string StatusClass
	    {
		    get
		    {
			    switch (Result)
			    {
				    case ExecutionResult.Crash:
				    case ExecutionResult.Timeout:
					    return "is-danger";
				    case ExecutionResult.Completed:
					    return "is-success";
				    default:
					    throw new ArgumentOutOfRangeException();
			    }
		    }
	    }
    }
}
