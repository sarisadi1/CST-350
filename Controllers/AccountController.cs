using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using App.Data; 

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }



   // handling the login form submission (POST)
[HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    // Validates the model
    if (!ModelState.IsValid)
    {
        ViewBag.ErrorMessage = "Both fields are required.";
        return View(model);
    }

    try
    {
        //  user by username
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

        // Check if user exists and if the password matches
        if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            // Setting the user’s status in a session variable
            HttpContext.Session.SetString("UserStatus", "LoggedIn");
            HttpContext.Session.SetString("Username", model.Username);

            
            HttpContext.Session.SetInt32("UserId", user.Id);

            // Redirect to the MainMenu page 
            return RedirectToAction("MainMenu");
        }
        else
        {
            // If credentials are incorrect---> set the error message
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View(model); 
        }
    }
    catch (Exception ex)
    {
        ViewBag.ErrorMessage = "An error occurred. Please try again later." + ex;
        return View(model);
    }
}

    [HttpGet]
        public IActionResult MainMenu()
        {
            return View();
        }

    //  success page if login is successful
    public IActionResult Success()
    {
        return View();
    }

    // error page if login fails
    public IActionResult Error()
    {
        return View();
    }

    // Logout user and clear session
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
