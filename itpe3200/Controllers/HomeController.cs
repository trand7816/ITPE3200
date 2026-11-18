using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using itpe3200.Models;
using Microsoft.AspNetCore.Authorization;
using itpe3200.DAL;
using itpe3200.ViewModels;
namespace itpe3200.Controllers;

public class HomeController : Controller
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ICourseRepository repo, ILogger<HomeController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // Welcome page with login. Logged-in teachers are sent straight to their view.
    // Also ensures that index /homepage will not return to login page throughout session. 
    public IActionResult Index()
    {
        if (User.IsInRole("Teacher"))
        { 
            return RedirectToAction(nameof(Teacher));
        }
        if (User.IsInRole("Student"))
        {
            return RedirectToAction("Join", "Group");  
        }
        return View();
    }

    // Teacher View: Only for logged-in users, not role-based
    [Authorize(Roles= "Teacher")]
    public async Task<IActionResult> Teacher()
    {
        var sessions = await _repo.GetAllSessions();
        var groups = await _repo.GetAllGroups();
        var students = await _repo.GetAllStudents();

        if (sessions is null || groups is null || students is null)
        {
            _logger.LogError("[HomeController] Could not load dashboard data");
        }

        string email = User.Identity?.Name ?? "";
        string name = email.Split('@')[0];

        var model = new TeacherHomeViewModel
        {
            Greeting = GetGreeting(DateTime.Now.Hour),
            TeacherName = name,
            SessionCount = sessions?.Count() ?? 0,
            GroupCount = groups?.Count() ?? 0,
            StudentCount = students?.Count() ?? 0,
            Sessions = sessions ?? new List<CourseSession>()
        };
        
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Log the unhandled exception that sent the user to this page
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (feature != null)
        {
            _logger.LogError(feature.Error, "[HomeController] Unhandled exception on path {Path}", feature.Path);
        }
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Supporting methods
    public static string GetGreeting(int hour)
    {
        if (hour < 12) return "Good morning";
        if (hour < 18) return "Good afternoon";
        return "Good evening";
    }
}

