using System;
using Microsoft.Extensions.DependencyInjection;
using WebpackTag.AssetParsers;

namespace WebpackTag
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers services required for the WebpackTag library.
		/// </summary>
		public static IServiceCollection AddWebpackTag(this IServiceCollection services, Action<WebpackTagOptions>? configure = null)
		{
			services.AddSingleton<IAssetParserFactory, AssetParserFactory>();
			services.AddSingleton<IAssetParser, AssetManifest>();
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
