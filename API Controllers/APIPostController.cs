using Microsoft.AspNetCore.Mvc;

namespace Social_Media.API_Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIPostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
