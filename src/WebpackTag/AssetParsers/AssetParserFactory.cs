using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WebpackTag.AssetParsers
{
	/// <summary>
	/// Handles constructing <see cref="IAssetParser"/> instances.
	/// </summary>
	public class AssetParserFactory : IAssetParserFactory
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IWebHostEnvironment _env;
		private readonly IEnumerable<IAssetParser> _availableParsers;
		private readonly IOptions<WebpackTagOptions> _options;
		private readonly IMemoryCache _cache;
		private readonly IHttpClientFactory _httpClientFactory;

		public AssetParserFactory(
			IServiceProvider serviceProvider,
			IWebHostEnvironment env,
			IEnumerable<IAssetParser> availableParsers,
			IOptions<WebpackTagOptions> options,
			IMemoryCache cache,
			IHttpClientFactory httpClientFactory
		)
		{
			_serviceProvider = serviceProvider;
			_env = env;
			_availableParsers = availableParsers;
			_options = options;
			_cache = cache;
			_httpClientFactory = httpClientFactory;
		}

		/// <summary>
		/// Gets the Webpack asset manifest using the correct asset parser.
		/// </summary>
		public Task<WebpackAssets> GetAssets()
		{
			if (_env.IsDevelopment())
			{
				// No caching in dev
				return GetAssetsImpl();
			}
			else
			{
				// This caches the actual Task<T> itself
				return _cache.GetOrCreate(nameof(AssetParserFactory), async entry => await GetAssetsImpl());
			}
		}

		private async Task<WebpackAssets> GetAssetsImpl()
		{
			var parserForPath = GetParserForPath();
			if (parserForPath != null)
			{
				var (parser, fileProvider, file) = parserForPath.Value;
				//entry.ExpirationTokens.Add(fileProvider.Watch(file.Name));
				var contents = File.ReadAllText(file.PhysicalPath);
				return parser.ParseManifest(contents);
			}

			var parserForUrl = GetParserForUrl();
			if (parserForUrl != null)
			{
				var (parser, url) = parserForUrl.Value;
				var contents = await _httpClientFactory.CreateClient().GetStringAsync(url);
				return parser.ParseManifest(contents);
			}

			throw new InvalidOperationException("Could not determine Webpack manifest type!");
		}

		/// <summary>
		/// If the manifest is located on disk, gets the parser used to parse it. Checks a few common paths.
		/// </summary>
		private (IAssetParser, IFileProvider, IFileInfo)? GetParserForPath()
		{
			var potentialProviders = new List<IFileProvider>
			{
				_env.WebRootFileProvider,
				_env.ContentRootFileProvider,
			};
			var spaProvider = _serviceProvider.GetService<ISpaStaticFileProvider>();
			if (spaProvider?.FileProvider != null)
			{
				potentialProviders.Add(spaProvider.FileProvider);
			}

			foreach (var provider in potentialProviders)
			{
				foreach (var parser in _availableParsers)
				{
					var fileInfo = provider.GetFileInfo(parser.DefaultFileName);
					if (fileInfo.Exists)
					{
						return (parser, provider, fileInfo);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// If the manifest is located at a URL (eg. using a dev server), gets the parser used to parse it.
		/// </summary>
		/// <returns></returns>
		private (IAssetParser, Uri)? GetParserForUrl()
		{
			if (!_env.IsDevelopment())
			{
				return null;
			}

			var parser = _availableParsers.First(x => x is AssetManifest);
			return (parser, new Uri($"http://localhost:{_options.Value.DevServerPort}/{parser.DefaultFileName}"));
		}
	}
}
