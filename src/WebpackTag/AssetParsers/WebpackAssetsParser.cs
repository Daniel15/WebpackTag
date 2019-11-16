﻿using System;
using System.Collections.Generic;
using System.Linq;
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
				var outputFiles = new Dictionary<string, List<string>>();
				output[entryPoint] = new WebpackAssets.EntryPoint
				{
					Files = outputFiles,
				};
				foreach (var (extension, files) in filesByExtension)
				{
					// This can be either an array (for multiple files), or a string (for one file)
					switch (files)
					{
						case JArray array:
							outputFiles[extension] = array.Select(x => x.ToString()).ToList();
							break;

						case string str:
							outputFiles[extension] = new List<string> { str };
							break;

						default:
							throw new ArgumentException("Unrecognised webpack-assets format");
					}
				}
			}

			return new WebpackAssets(output);
		}
	}
}
