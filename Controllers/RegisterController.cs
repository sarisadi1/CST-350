using Microsoft.AspNetCore.Mvc;
using App.Data; 
// using App.Model;
public class RegisterController : Controller
{
    private readonly ApplicationDbContext _context;

    public RegisterController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Sex = model.Sex,
                Age = model.Age,
                State = model.State,
                Email = model.Email,
                Username = model.Username,
                Password = hashedPassword 
            };

            _context.Users.Add(user);
            // Save asynchronously
            await _context.SaveChangesAsync();  
            return RedirectToAction("Success");
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }


    public IActionResult Success()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
