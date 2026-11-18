using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace itpe3200.Controllers;

// CRUD for students. A student can be a member of several groups
// (one per course session) through the GroupMember join table.
[Authorize(Roles = "Teacher")]
public class StudentController : Controller
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<StudentController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public StudentController(ICourseRepository repo, ILogger<StudentController> logger, UserManager<IdentityUser> userManager)
    {
        _repo = repo;
        _logger = logger;
        _userManager = userManager;
    }

    // GET: Student – lists all students
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var students = await _repo.GetAllStudents();
        if (students is null)
        {
            _logger.LogError("[StudentController] Students could not be loaded");
            return Problem("Unable to load students.");
        }

        var model = students.Select(s => new StudentListItemViewModel
        {
            Id = s.Id,
            Name = s.Name,
            GroupCount = s.Groups.Count
        });

        return View(model);
    }

    // GET: Student/Details/5 – shows the student and the groups they belong to
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var student = await _repo.GetStudentById(id);
        if (student is null)
        {
            _logger.LogWarning("[StudentController] Student not found for StudentId {StudentId}", id);
            return NotFound("Student not found.");
        }

        var model = new StudentDetailsViewModel
        {
            Id = student.Id,
            Name = student.Name,
            Groups = student.Groups
                .Where(member => member.Group is not null)
                .Select(member => new StudentGroupViewModel
                {
                    Id = member.Group!.Id,
                    Name = member.Group.Name ?? "",
                    SessionName = member.Group.CourseSession?.Name ?? "",
                    JoinedAt = member.JoinedAt
                })
                .ToList()
        };

        return View(model);
    }

    // GET: Student/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new StudentCreateViewModel());
    }

    // POST: Student/Create
    [HttpPost]
    public async Task<IActionResult> Create(StudentCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[StudentController] Student creation failed validation {@model}", model);
            return View(model);
        }

        var student = new Student { Name = model.Name.Trim() };

        if (!await _repo.AddStudent(student))
        {
            // The repository has already logged the database error
            _logger.LogError("[StudentController] Student creation failed for {@student}", student);
            ModelState.AddModelError("", "Could not save the student. Please try again.");
            return View(model);
        }

        _logger.LogInformation("[StudentController] Student {StudentName} created", student.Name);
        return RedirectToAction(nameof(Index));
    }

    // GET: Student/Update/5
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var student = await _repo.GetStudentById(id);
        if (student is null)
        {
            _logger.LogWarning("[StudentController] Student not found when updating StudentId {StudentId}", id);
            return NotFound("Student not found.");
        }

        var model = new StudentUpdateViewModel
        {
            Id = student.Id,
            Name = student.Name
        };

        return View(model);
    }

    // POST: Student/Update/5
    [HttpPost]
    public async Task<IActionResult> Update(StudentUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[StudentController] Student update failed validation for StudentId {StudentId}", model.Id);
            return View(model);
        }

        // Load the saved student first, so the group memberships are kept
        var student = await _repo.GetStudentById(model.Id);
        if (student is null)
        {
            _logger.LogWarning("[StudentController] Student not found when updating StudentId {StudentId}", model.Id);
            return NotFound("Student not found.");
        }

        student.Name = model.Name.Trim();

        if (!await _repo.UpdateStudent(student))
        {
            _logger.LogError("[StudentController] Student update failed for StudentId {StudentId}", model.Id);
            ModelState.AddModelError("", "Could not update the student.");
            return View(model);
        }

        _logger.LogInformation("[StudentController] Student {StudentId} updated", student.Id);
        return RedirectToAction(nameof(Details), new { id = student.Id });
    }

    // GET: Student/Delete/5 – shows a confirmation page before deleting
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _repo.GetStudentById(id);
        if (student is null)
        {
            _logger.LogWarning("[StudentController] Student not found when deleting StudentId {StudentId}", id);
            return NotFound("Student not found.");
        }

        var model = new StudentDeleteViewModel
        {
            Id = student.Id,
            Name = student.Name,
            Groups = student.Groups
                .Where(member => member.Group is not null)
                .Select(member => new StudentGroupViewModel
                {
                    Id = member.Group!.Id,
                    Name = member.Group.Name ?? "",
                    SessionName = member.Group.CourseSession?.Name ?? "",
                    JoinedAt = member.JoinedAt
                })
                .ToList()
        };

        return View(model);
    }

    // POST: Student/DeleteConfirmed/5 - deletes the student profile as well as the login account
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Get the student first, so we know which login account belongs to it
        var student = await _repo.GetStudentById(id);
        if (student is null)
        {
            _logger.LogWarning("[StudentController] Student not found when deleting StudentId {StudentId}", id);
            return NotFound("Student not found.");
        }

        if (!await _repo.DeleteStudent(id))
        {
            _logger.LogError("[StudentController] Student deletion failed for StudentId {StudentId}", id);
            return BadRequest("Student could not be deleted.");
        }

        // Also delete the student's login account, so they can no longer log in.
        // Seeded students have no account (IdentityUserId is null).
        if (student.IdentityUserId is not null)
        {
            var user = await _userManager.FindByIdAsync(student.IdentityUserId);
            if (user is not null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("[StudentController] Login account could not be deleted for StudentId {StudentId}", id);
                }
            }
        }

        _logger.LogInformation("[StudentController] Student {StudentId} deleted", id);
        return RedirectToAction(nameof(Index));
    }
}
