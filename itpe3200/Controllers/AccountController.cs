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
    private readonly ICourseRepository _repo;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ILogger<AccountController> logger,
        ICourseRepository repo)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _repo = repo;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    // Registration has three steps: create the login account, give it the Student role,
    // and create the linked student profile. If step 2 or 3 fails, the account is deleted
    // again, so we never end up with an account that has no role or no student profile.
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Step 1: create the login account
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            // Show Identity's own messages (weak password, email already taken, and so on)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogWarning("[AccountController] Registration failed for {Email}", model.Email);
            return View(model);
        }

        // Step 2: all new users get the Student role (teachers are seeded, not registered)
        var roleResult = await _userManager.AddToRoleAsync(user, "Student");
        if (!roleResult.Succeeded)
        {
            _logger.LogError("[AccountController] Could not give the Student role to {Email}", model.Email);
            return await UndoRegistration(user, model);
        }

        // Step 3: create the student profile, linked to the login account through IdentityUserId
        var student = new Student
        {
            Name = model.Name.Trim(),
            IdentityUserId = user.Id
        };

        // AddStudent has its own try/catch and logging, and returns false on failure
        if (!await _repo.AddStudent(student))
        {
            _logger.LogError("[AccountController] Could not create a student profile for {Email}", model.Email);
            return await UndoRegistration(user, model);
        }

        _logger.LogInformation("[AccountController] New user registered: {Email}", model.Email);

        // Log the new user in straight away
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    // Deletes a half-finished account so the user can try to register again with the same email
    private async Task<IActionResult> UndoRegistration(IdentityUser user, RegisterViewModel model)
    {
        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            _logger.LogError("[AccountController] Could not remove incomplete account {Email}", model.Email);
        }

        ModelState.AddModelError(string.Empty, "Registration could not be completed. Please try again.");
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

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}