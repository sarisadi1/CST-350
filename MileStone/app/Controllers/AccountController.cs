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

    // Display the login form (GET)
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }



   // Handle the login form submission (POST)
[HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    // Validate the model
    if (!ModelState.IsValid)
    {
        ViewBag.ErrorMessage = "Both fields are required.";
        return View(model);
    }

    try
    {
        // Find the user by username
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

        // Check if user exists and if the password matches
        if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            // Set the user’s status in a session variable
            HttpContext.Session.SetString("UserStatus", "LoggedIn");
            HttpContext.Session.SetString("Username", model.Username);

            // Optionally, you can also store other user-specific data in the session if needed
            HttpContext.Session.SetInt32("UserId", user.Id);

            // Redirect to the success page (or dashboard)
            return RedirectToAction("Success");
        }
        else
        {
            // If credentials are incorrect, set the error message
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View(model); // Re-render the view with the error
        }
    }
    catch (Exception ex)
    {
        // Log the exception (use logging framework in production)
        ViewBag.ErrorMessage = "An error occurred. Please try again later.";
        return View(model);
    }
}


    // Display success page if login is successful
    public IActionResult Success()
    {
        return View();
    }

    // Display error page if login fails
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
