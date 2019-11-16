using Microsoft.AspNetCore.Mvc;

namespace WebpackTag.Samples.WebpackAssets.Controllers
{
	public class HomeController : Controller
	{
		[Route("")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
