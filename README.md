# WebpackTag

[![NuGet version](http://img.shields.io/nuget/v/WebpackTag.svg)](https://www.nuget.org/packages/WebpackTag/)

WebpackTag is an [ASP.NET Core Tag Helper](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-3.0) for rendering links to CSS and JS files compiled via Webpack

* Supports both [webpack-manifest-plugin](https://www.npmjs.com/package/webpack-manifest-plugin) (as used by [Create React App](https://create-react-app.dev/)) and [assets-webpack-plugin](https://www.npmjs.com/package/assets-webpack-plugin).
* Supports loading manifest from Webpack devserver in development.
* Renders [preload headers](https://developer.mozilla.org/en-US/docs/Web/HTML/Preloading_content) for improved performance. This can be used to support [HTTP/2 Server Push in Nginx](https://www.nginx.com/blog/nginx-1-13-9-http2-server-push/#automatic-push).

# Usage

Install the  `WebpackTag` library using NuGet

Add the WebpackTag services to your `Startup.cs`:

```csharp
using WebpackTag;
...
public void ConfigureServices(IServiceCollection services)
{
	services.AddWebpackTag();
```

Import the tag helpers in your `Views/_ViewImports.cshtml` file:

```csharp
@addTagHelper *, WebpackTag
```

Then use the tag helpers in your view!

```html
<webpack-styles />
<webpack-scripts />
```

These tag helpers will look for files called `webpack-assets.json` or `asset-manifest.json` in your wwwroot or SPA root directory, parse the contents, and render the correct `<link>` or `<style>` tags.

Note that when using `assets-webpack-plugin`, the `entrypoints` option should be **enabled**.

## Multiple entry points

If your app has multiple entry points, you may specify the entry point name when using the tag

```html
<webpack-styles entry="first" />
```

## Configuration

Additional configuration can be performed when registering the services:

```csharp
services.AddWebpackTag(options =>
{
	// ...
});
```

The following configuration options are available:

* `DevServerPort`: Port the Webpack devserver is running on. If this is configured, the tag helpers will try hitting `http://localhost:{port}/asset-manifest.json` to load the manifest.
* `BaseUrl`: Sets a string to prefix the generated URLs with. For example, this can be used to use a separate CDN domain in prod:

```csharp
public class Startup
{
	private readonly IWebHostEnvironment _env;

	public Startup(IConfiguration configuration, IWebHostEnvironment env)
	{
		_env = env;
		Configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddWebpackTag(options =>
		{
			options.BaseUrl = _env.IsDevelopment() ? "/" : "https://cdn.example.com/";
		});
		// ...
```

# Samples

This repository contains two samples:

* `WebpackTag.Samples.React`: Uses the [ASP.NET Core 3.0 React project template](https://docs.microsoft.com/en-us/aspnet/core/client-side/spa/react?view=aspnetcore-3.0&tabs=visual-studio), which uses Create React App.
* `WebpackTag.Samples.WebpackAssets`: Basic example using [assets-webpack-plugin](https://www.npmjs.com/package/assets-webpack-plugin).
