using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebpackTag
{
	public class WebpackAssets
	{
		private readonly ImmutableDictionary<string, EntryPoint> _entryPoints;

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

		public class EntryPoint
		{
			public ImmutableDictionary<string, ImmutableArray<string>> Files { get; }

			public EntryPoint(ImmutableDictionary<string, ImmutableArray<string>> files)
			{
				Files = files;
			}
		}
	}
}
