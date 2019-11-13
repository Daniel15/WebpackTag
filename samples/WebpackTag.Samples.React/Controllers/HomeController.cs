using Microsoft.AspNetCore.Mvc;

namespace WebpackTag.Samples.React.Controllers
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
