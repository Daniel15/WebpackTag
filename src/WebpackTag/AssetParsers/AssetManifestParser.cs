using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Parses <c>asset-manifest.json</c> files generated from the Webpack
	/// <c>webpack-assets-manifest</c> plugin. In production, the file is read from disk.
	/// In dev, the file is read from the Webpack dev server.
	/// </summary>
	public class AssetManifestParser : IAssetParser
	{
		private readonly IOptions<WebpackTagOptions> _options;

		/// <summary>
		/// Creates a new <see cref="AssetManifestParser"/>.
		/// </summary>
		public AssetManifestParser(IOptions<WebpackTagOptions> options)
		{
			_options = options;
		}

		/// <summary>
		/// Default file name for this manifest type (eg. "asset-manifest.json")
		/// </summary>
		public string DefaultFileName => "asset-manifest.json";

		/// <summary>
		/// Parses the manifest file
		/// </summary>
		/// <param name="json">Contents of the file</param>
		/// <returns>Parsed Webpack assets</returns>
		public WebpackAssets ParseManifest(string json)
		{
			var manifest = JsonSerializer.Deserialize<ManifestContent>(json, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			});
			var files = manifest.Entrypoints
				.Select(path => _options.Value.BaseUrl + path)
				.GroupBy(path => (Path.GetExtension(path) ?? "").TrimStart('.'))
				.ToDictionary(x => x.Key, x => x.ToImmutableArray())
				.ToImmutableDictionary();


			return new WebpackAssets(new Dictionary<string, WebpackAssets.EntryPoint>
			{
				{"", new WebpackAssets.EntryPoint(files)}
			}.ToImmutableDictionary());
		}

		/// <summary>
		/// POCO for asset-manifest.json file
		/// </summary>
		class ManifestContent
		{
			/// <summary>
			/// Gets or sets the files required by the entry point.
			/// </summary>
			public IList<string> Entrypoints { get; set; } = new List<string>();
		}
	}
}
