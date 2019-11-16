using System;
using System.Collections.Generic;

namespace WebpackTag
{
	public class WebpackAssets
	{
		private readonly IDictionary<string, EntryPoint> _entryPoints;

		public WebpackAssets(IDictionary<string, EntryPoint> entryPoints)
		{
			_entryPoints = entryPoints;
		}

		/// <summary>
		/// Gets all the files required by the specified entrypoint.
		/// </summary>
		public IList<string> GetPaths(string extension, string entryPoint = "")
		{
			if (!_entryPoints.ContainsKey(entryPoint))
			{
				throw new ArgumentException($"Invalid entrypoint '{entryPoint}'", nameof(entryPoint));
			}

			var files = _entryPoints[entryPoint].Files;
			return files.ContainsKey(extension) ? files[extension] : new List<string>();
		}

		public class EntryPoint
		{
			public Dictionary<string, List<string>> Files { get; set; } = new Dictionary<string, List<string>>();
		}
	}
}
