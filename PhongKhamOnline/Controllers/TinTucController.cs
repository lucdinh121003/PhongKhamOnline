using Microsoft.AspNetCore.Mvc;

namespace PhongKhamOnline.Controllers
{
    public class TinTucController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
