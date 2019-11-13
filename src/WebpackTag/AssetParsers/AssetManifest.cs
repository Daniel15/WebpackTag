using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Parses <c>asset-manifest.json</c> files generated from the Webpack
	/// <c>webpack-assets-manifest</c> plugin. In production, the file is read from disk.
	/// In dev, the file is read from the Webpack dev server.
	/// </summary>
	public class AssetManifest : IWebpackAssets
	{
		private readonly IWebHostEnvironment _env;
		private readonly IOptions<WebpackTagOptions> _options;

		/// <summary>
		/// Client used to hit the dev server
		/// </summary>
		private static readonly HttpClient _client = new HttpClient();

		/// <summary>
		/// Files loaded from the manifest. Only used in production.
		/// </summary>
		private Dictionary<string, List<string>> _files = new Dictionary<string, List<string>>();

		public AssetManifest(
			IWebHostEnvironment env, 
			ISpaStaticFileProvider fileProvider,
			IOptions<WebpackTagOptions> options
		)
		{
			_env = env;
			_options = options;

			if (!env.IsDevelopment())
			{
				var json = File.ReadAllText(
					fileProvider.FileProvider.GetFileInfo("asset-manifest.json").PhysicalPath
				);
				_files = ParseManifest(json);
			}
		}

		/// <summary>
		/// Gets all the files required by the specified entrypoint.
		/// </summary>
		public async ValueTask<IList<string>> GetPaths(string extension, string entryPoint = "")
		{
			if (entryPoint != "")
			{
				throw new ArgumentException("asset-manifest only has one entry point", nameof(entryPoint));
			}

			var files = _env.IsProduction()
				? _files
				: await LoadDevManifest();

			return files.ContainsKey(extension) ? files[extension] : new List<string>();
		}

		/// <summary>
		/// Loads the manifest from the development server.
		/// </summary>
		/// <returns></returns>
		private async ValueTask<Dictionary<string, List<string>>> LoadDevManifest()
		{
			var manifestUri = new Uri($"http://localhost:{_options.Value.DevServerPort ?? 9000}/asset-manifest.json");
			var json = await _client.GetStringAsync(manifestUri);
			return ParseManifest(json);
		} 

		/// <summary>
		/// Parses the manifest file
		/// </summary>
		private Dictionary<string, List<string>> ParseManifest(string json)
		{
			var manifest = JsonSerializer.Deserialize<ManifestContent>(json, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
			return manifest.Entrypoints
				.Select(path => "/" + path)
				.GroupBy(path => Path.GetExtension(path) ?? "")
				.ToDictionary(x => x.Key, x => x.ToList());
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
