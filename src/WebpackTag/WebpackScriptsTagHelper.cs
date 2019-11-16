using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebpackTag.AssetParsers;

namespace WebpackTag
{
	/// <summary>
	/// Renders references to Webpack-compiled JavaScript files
	/// </summary>
	[OutputElementHint("script")]
	public class WebpackScriptsTagHelper : TagHelper
	{
		private const string EXTENSION = "js";

		private readonly IAssetParserFactory _parser;
		private readonly IHttpContextAccessor _httpContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebpackScriptsTagHelper"/> class.
		/// </summary>
		/// <param name="parser">The parser factory.</param>
		/// <param name="httpContext">The HTTP context.</param>
		public WebpackScriptsTagHelper(IAssetParserFactory parser, IHttpContextAccessor httpContext)
		{
			_parser = parser;
			_httpContext = httpContext;
		}

		/// <summary>
		/// Webpack entry point name
		/// </summary>
		public string Entry { get; set; } = "";

		/// <inheritdoc />
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;

			var assets = await _parser.GetAssets();
			var scripts = assets.GetPaths(EXTENSION, Entry);
			foreach (var script in scripts)
			{
				output.Content.AppendHtml(@"<script src=""");
				output.Content.Append(script);
				output.Content.AppendHtmlLine(@"""></script>");
			}

			_httpContext.HttpContext.Response.Headers.AppendCommaSeparatedValues(
				"Link",
				scripts.Select(script => $"<{script}>; as=script; rel=preload").ToArray()
			);
		}
	}
}
