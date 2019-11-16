using System;
using System.IO;
using Microsoft.Extensions.Options;
using WebpackTag.AssetParsers;
using Xunit;

namespace WebpackTag.Tests.AssetParsers
{
	public class AssetManifestParserTest
	{
		[Fact]
		public void ParsesAssetManifest()
		{
			var parser = new AssetManifestParser(new OptionsWrapper<WebpackTagOptions>(new WebpackTagOptions()));
			var result = parser.ParseManifest(Fixture1);

			var jsPaths = result.GetPaths("js");
			Assert.Equal(new[]
			{
				"/static/js/runtime-main.d1ae9948.js",
				"/static/js/2.a6664501.chunk.js",
				"/static/js/main.a7ba1e53.chunk.js"
			}, jsPaths);

			var cssPaths = result.GetPaths("css");
			Assert.Equal(new[]
			{
				"/static/css/2.833dd627.chunk.css",
				"/static/css/main.13b298a8.chunk.css",
			}, cssPaths);

			var fooPaths = result.GetPaths("foo");
			Assert.Empty(fooPaths);
		}

		[Fact]
		public void PrependsPath()
		{
			var parser = new AssetManifestParser(new OptionsWrapper<WebpackTagOptions>(new WebpackTagOptions
			{
				BaseUrl = "https://cdn.example.com/"
			}));
			var result = parser.ParseManifest(Fixture1);

			var jsPaths = result.GetPaths("js");
			Assert.Equal(new[]
			{
				"https://cdn.example.com/static/js/runtime-main.d1ae9948.js",
				"https://cdn.example.com/static/js/2.a6664501.chunk.js",
				"https://cdn.example.com/static/js/main.a7ba1e53.chunk.js"
			}, jsPaths);
		}

		[Fact]
		public void ThrowsOnOtherEntryPoint()
		{
			var parser = new AssetManifestParser(new OptionsWrapper<WebpackTagOptions>(new WebpackTagOptions()));
			var result = parser.ParseManifest(Fixture1);
			Assert.Throws<ArgumentException>(() => result.GetPaths("js", "foobar"));
		}

		private string Fixture1
		{
			get
			{
				var path = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "asset-manifest1.json");
				return File.ReadAllText(path);
			}
		}
	}
}
