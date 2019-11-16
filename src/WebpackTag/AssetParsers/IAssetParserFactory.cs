using System.Threading.Tasks;

namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Handles constructing <see cref="IAssetParser"/> instances.
	/// </summary>
	public interface IAssetParserFactory
	{
		/// <summary>
		/// Gets the Webpack asset manifest using the correct asset parser.
		/// </summary>
		Task<WebpackAssets> GetAssets();
	}
}
