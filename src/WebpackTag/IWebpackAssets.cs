using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebpackTag
{
	/// <summary>
	/// Retrieves data for assets compiled via Webpack
	/// </summary>
	public interface IWebpackAssets
	{
		/// <summary>
		/// Gets all the files required by the specified entrypoint.
		/// </summary>
		ValueTask<IList<string>> GetPaths(string extension, string entryPoint = "");
	}
}
