namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Parser for Webpack assets
	/// </summary>
	public interface IAssetParser
	{
		/// <summary>
		/// Default file name for this manifest type (eg. "asset-manifest.json")
		/// </summary>
		string DefaultFileName { get; }

		/// <summary>
		/// Parses the manifest file
		/// </summary>
		/// <param name="json">Contents of the file</param>
		/// <returns>Parsed Webpack assets</returns>
		WebpackAssets ParseManifest(string json);
	}
}
