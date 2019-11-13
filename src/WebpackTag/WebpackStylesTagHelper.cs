using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebpackTag
{
	/// <summary>
	/// Renders references to Webpack-compiled CSS files
	/// </summary>
	[OutputElementHint("link")]
	public class WebpackStylesTagHelper : TagHelper
	{
		private const string EXTENSION = ".css";

		private readonly IWebpackAssets _assets;
		private readonly IHttpContextAccessor _httpContext;

		public WebpackStylesTagHelper(IWebpackAssets assets, IHttpContextAccessor httpContext)
		{
			_assets = assets;
			_httpContext = httpContext;
		}

		/// <summary>
		/// Webpack entry point name
		/// </summary>
		public string Entry { get; set; } = "";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;

			var stylesheets = await _assets.GetPaths(EXTENSION, Entry);
			foreach (var stylesheet in stylesheets)
			{
				output.Content.AppendHtml(@"<link rel=""stylesheet"" href=""");
				output.Content.Append(stylesheet);
				output.Content.AppendHtmlLine(@""">");
			}

			_httpContext.HttpContext.Response.Headers.AppendCommaSeparatedValues(
				"Link",
				stylesheets.Select(stylesheet => $"<{stylesheet}>; as=style; rel=preload").ToArray()
			);
		}
	}
}
