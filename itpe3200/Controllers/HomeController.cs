using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using itpe3200.Models;
using Microsoft.AspNetCore.Authorization;
using itpe3200.DAL;
using itpe3200.ViewModels;
using System.Security.Claims;
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

    // Welcome page with login. Logged-in users are sent to their role’s homepage.
    // Also ensures that index /homepage will not return to login page throughout session. 
    public IActionResult Index()
    {
        if (User.IsInRole("Teacher"))
        {
            return RedirectToAction(nameof(Teacher));
        }
        if (User.IsInRole("Student"))
        {
            return RedirectToAction(nameof(Student));
        }
        return View();
    }

    // Teacher homepage: only for users with teacher role
    [Authorize(Roles = "Teacher")]
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

    //Student homepage: greeting, join form and the student's groups
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Student()
    {
        var groups = new List<StudentGroupViewModel>();

        //Fetch the logged-in user's id
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //Fetch the user profile connected to the login account as they are separate records
        var profile = userId is null ? null : await _repo.GetStudentByIdentityUserId(userId);

        if (profile is null)
        {
            _logger.LogWarning("[HomeController] No student profile found for the logged-in user");
        }
        else
        {
            //GetStudentbyId also loads the student's groups and course sessions 
            var student = await _repo.GetStudentById(profile.Id);
            if (student is not null)
            {
                groups = student.Groups
                .Where(member => member.Group is not null)
                .Select(member => new StudentGroupViewModel
                {
                    Id = member.Group!.Id,
                    Name = member.Group.Name ?? "",
                    SessionName = member.Group.CourseSession?.Name ?? "",
                    JoinedAt = member.JoinedAt
                })
               .ToList();
            }
        }

        string email = User.Identity?.Name ?? "";

        var model = new StudentHomeViewModel
        {
            Greeting = GetGreeting(DateTime.Now.Hour),
            StudentName = profile?.Name.Split(' ')[0] ?? "",
            Groups = groups
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

