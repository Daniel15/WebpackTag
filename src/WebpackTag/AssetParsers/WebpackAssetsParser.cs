﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Parses <c>webpack-assets.json</c> files generated from the Webpack
	/// <c>assets-webpack-plugin</c> plugin.
	/// </summary>
	public class WebpackAssetsParser : IAssetParser
	{
		private readonly IOptions<WebpackTagOptions> _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebpackAssetsParser"/> class.
		/// </summary>
		/// <param name="options">The options.</param>
		public WebpackAssetsParser(IOptions<WebpackTagOptions> options)
		{
			_options = options;
		}

		/// <summary>
		/// Default file name for this manifest type (eg. "asset-manifest.json")
		/// </summary>
		public string DefaultFileName => "webpack-assets.json";

		/// <summary>
		/// Parses the manifest file
		/// </summary>
		/// <param name="json">Contents of the file</param>
		/// <returns>Parsed Webpack assets</returns>
		public WebpackAssets ParseManifest(string json)
		{
			var manifest = JsonConvert.DeserializeObject<IDictionary<string, IDictionary<string, dynamic>>>(json);

			var output = new Dictionary<string, WebpackAssets.EntryPoint>();
			foreach (var (entryPoint, filesByExtension) in manifest)
			{
				var outputFiles = new Dictionary<string, ImmutableArray<string>>();
				foreach (var (extension, files) in filesByExtension)
				{
					// This can be either an array (for multiple files), or a string (for one file)
					switch (files)
					{
						case JArray array:
							outputFiles[extension] = array.Select(x => FormatPath(x.ToString())).ToImmutableArray();
							break;

						case string str:
							outputFiles[extension] = ImmutableArray.Create(FormatPath(str));
							break;

						default:
							throw new ArgumentException("Unrecognised webpack-assets format");
					}
				}

				output[entryPoint] = new WebpackAssets.EntryPoint(outputFiles.ToImmutableDictionary());
			}

			return new WebpackAssets(output.ToImmutableDictionary());
		}

		private string FormatPath(string path)
		{
			return _options.Value.BaseUrl + path.TrimStart('/');
		}
	}
}
