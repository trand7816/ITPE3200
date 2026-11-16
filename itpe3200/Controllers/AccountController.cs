using itpe3200.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using itpe3200.DAL;
using itpe3200.Models;

namespace itpe3200.Controllers;

// Handles registration, login and logout. User accounts are stored in the
// Identity tables (AspNetUsers), which are separate from our domain entities.
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly AppDbContext _db;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<AccountController> logger,
        AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _db = db;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            //If account creation succeeds, the user is automatically assigned a Student role
            //(Does not apply to excisting accounts)
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Student");
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                _logger.LogInformation("[AccountController] New user registered: {Email}", model.Email);

                //Registers the users identityID in db after creation
                var student = new Student
                {
                    Name = model.Email,
                    IdentityUserId = user.Id
                };
                
                _db.Students.Add(student);
                await _db.SaveChangesAsync();

                // Log the new user in straight away
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Show Identity's own messages (weak password, email already taken, and so on)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogWarning("[AccountController] Registration failed for {Email}", model.Email);
        }
        return View(model);
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        var model = new LoginViewModel { ReturnUrl = returnUrl };
        return View(model);
    }

    // POST: /Account/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Only allow local URLs, so the app cannot be used to redirect to another site
        string targetUrl = !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : Url.Action("Index", "Home") ?? "~/";

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("[AccountController] User {Email} logged in", model.Email);
                return Redirect(targetUrl);
            }

            // Do not say whether it was the email or the password that was wrong
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            _logger.LogWarning("[AccountController] Failed login attempt for {Email}", model.Email);
        }

        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("[AccountController] User {Email} logged out", User.Identity?.Name);
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}