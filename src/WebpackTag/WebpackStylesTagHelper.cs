using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebpackTag.AssetParsers;

namespace WebpackTag
{
	/// <summary>
	/// Renders references to Webpack-compiled CSS files
	/// </summary>
	[OutputElementHint("link")]
	public class WebpackStylesTagHelper : TagHelper
	{
		private const string EXTENSION = ".css";

		private readonly IAssetParserFactory _parser;
		private readonly IHttpContextAccessor _httpContext;

		public WebpackStylesTagHelper(IAssetParserFactory parser, IHttpContextAccessor httpContext)
		{
			_parser = parser;
			_httpContext = httpContext;
		}

		/// <summary>
		/// Webpack entry point name
		/// </summary>
		public string Entry { get; set; } = "";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;

			var assets = await _parser.GetAssets();
			var stylesheets = assets.GetPaths(EXTENSION, Entry);
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
