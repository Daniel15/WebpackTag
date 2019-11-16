using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Parses <c>asset-manifest.json</c> files generated from the Webpack
	/// <c>webpack-assets-manifest</c> plugin. In production, the file is read from disk.
	/// In dev, the file is read from the Webpack dev server.
	/// </summary>
	public class AssetManifestParser : IAssetParser
	{
		/// <summary>
		/// Default file name for this manifest type (eg. "asset-manifest.json")
		/// </summary>
		public string DefaultFileName => "asset-manifest.json";

		/// <summary>
		/// Parses the manifest file
		/// </summary>
		/// <param name="contents">Contents of the file</param>
		/// <returns>Parsed Webpack assets</returns>
		public WebpackAssets ParseManifest(string json)
		{
			var manifest = JsonSerializer.Deserialize<ManifestContent>(json, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
			});
			var files = manifest.Entrypoints
				.Select(path => "/" + path)
				.GroupBy(path => (Path.GetExtension(path) ?? "").TrimStart('.'))
				.ToDictionary(x => x.Key, x => x.ToList());

			return new WebpackAssets(new Dictionary<string, WebpackAssets.EntryPoint>
				{
					{
						"", new WebpackAssets.EntryPoint
						{
							Files = files,
						}
					},
				}
			);
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
