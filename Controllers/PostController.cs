using Microsoft.AspNetCore.Mvc;

namespace Social_Media.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
