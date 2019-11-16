using System;
using Microsoft.Extensions.DependencyInjection;
using WebpackTag.AssetParsers;

namespace WebpackTag
{
	/// <summary>
	/// Handles registration of WebpackTag into the dependency injection container.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers services required for the WebpackTag library.
		/// </summary>
		public static IServiceCollection AddWebpackTag(this IServiceCollection services, Action<WebpackTagOptions>? configure = null)
		{
			services.AddSingleton<IAssetParserFactory, AssetParserFactory>();
			services.AddSingleton<IAssetParser, AssetManifestParser>();
			services.AddSingleton<IAssetParser, WebpackAssetsParser>();
			services.AddHttpClient();
			services.AddHttpContextAccessor();

			if (configure != null)
			{
				services.Configure(configure);
			}

			return services;
		}
	}
}
