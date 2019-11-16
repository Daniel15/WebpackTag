namespace WebpackTag
{
	/// <summary>
	/// Options for the WebpackTag library
	/// </summary>
	public class WebpackTagOptions
	{
		/// <summary>
		/// Gets or sets the base URL to prepend to asset URLs
		/// </summary>
		public string BaseUrl { get; set; } = "/";

		/// <summary>
		/// Gets or sets the port the devserver is running on.
		/// </summary>
		public int? DevServerPort { get; set; }
	}
}
