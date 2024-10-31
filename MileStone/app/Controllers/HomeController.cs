using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
     public IActionResult Rules()
        {
            return View();
        }
     public IActionResult Privacy()
        {
            return View();
        }
}
