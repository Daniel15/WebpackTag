using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebpackTag
{
	/// <summary>
	/// Contains details about assets compiled by Webpack
	/// </summary>
	public class WebpackAssets
	{
		private readonly ImmutableDictionary<string, EntryPoint> _entryPoints;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebpackAssets"/> class.
		/// </summary>
		/// <param name="entryPoints">The Webpack entry points.</param>
		public WebpackAssets(ImmutableDictionary<string, EntryPoint> entryPoints)
		{
			_entryPoints = entryPoints;
		}

		/// <summary>
		/// Gets all the files required by the specified entrypoint.
		/// </summary>
		public IReadOnlyList<string> GetPaths(string extension, string entryPoint = "")
		{
			if (!_entryPoints.ContainsKey(entryPoint))
			{
				throw new ArgumentException($"Invalid entrypoint '{entryPoint}'", nameof(entryPoint));
			}

			var files = _entryPoints[entryPoint].Files;
			return files.ContainsKey(extension) ? files[extension] : ImmutableArray<string>.Empty;
		}

		/// <summary>
		/// Represents a Webpack entry point
		/// </summary>
		public class EntryPoint
		{
			/// <summary>
			/// Gets the files required by this entry point.
			/// </summary>
			public ImmutableDictionary<string, ImmutableArray<string>> Files { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="EntryPoint"/> class.
			/// </summary>
			/// <param name="files">The files in this entry point.</param>
			public EntryPoint(ImmutableDictionary<string, ImmutableArray<string>> files)
			{
				Files = files;
			}
		}
	}
}
