using System;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Janitra.TagHelpers
{
	public class AccuracyStatusTagHelper : TagHelper
	{
		public AccuracyStatus Status { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetContent(Status.ToString());
			output.Attributes.SetAttribute("class", "tag is-medium " + StatusClass);
		}

		private string StatusClass
		{
			get
			{
				switch (Status)
				{
					case AccuracyStatus.Unset:
						return "is-warning";
					case AccuracyStatus.Correct:
						return "is-success";
					case AccuracyStatus.Incorrect:
						return "is-danger";
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}