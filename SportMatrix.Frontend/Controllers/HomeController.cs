using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Athletes");

            return View();
        }

        return View();
    }
}
