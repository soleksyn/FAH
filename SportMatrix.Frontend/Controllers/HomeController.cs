using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Athletes");
    }
}
