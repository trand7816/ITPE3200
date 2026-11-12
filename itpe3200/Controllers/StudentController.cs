using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace itpe3200.Controllers;

public class StudentController : Controller
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<StudentController> _logger;

    public StudentController(
        ICourseRepository repo,
        ILogger<StudentController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // Shows all students. The repository also loads their group memberships
    // so the list can be extended with group information later.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var students = await _repo.GetAllStudents();

        if (students is null)
        {
            _logger.LogError("[StudentController] Could not load students");
            return Problem("Could not load students.");
        }

        return View(students);
    }

    // Shows one student and the groups linked through the GroupMember table.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var student = await _repo.GetStudentById(id);

        if (student is null)
        {
            _logger.LogWarning(
                "[StudentController] Student not found for StudentId {StudentId}",
                id);

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
                    SessionName = member.Group.CourseSession?.Name ?? ""
                })
                .ToList()
        };

        return View(model);
    }

    // Shows the edit form. Only authenticated users may update students.
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        var student = await _repo.GetStudentById(id);

        if (student is null)
        {
            return NotFound("Student not found.");
        }

        var model = new StudentUpdateViewModel
        {
            Id = student.Id,
            Name = student.Name
        };

        return View(model);
    }

    // Validates and saves the values submitted from the edit form.
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(StudentUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var student = await _repo.GetStudentById(model.Id);

        if (student is null)
        {
            return NotFound("Student not found.");
        }

        student.Name = model.Name.Trim();

        if (!await _repo.UpdateStudent(student))
        {
            ModelState.AddModelError(
                "",
                "Could not update the student. Please try again.");

            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = student.Id });
    }

    // Shows a confirmation page before deleting a student.
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _repo.GetStudentById(id);

        if (student is null)
        {
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
                    SessionName = member.Group.CourseSession?.Name ?? ""
                })
                .ToList()
        };

        return View(model);
    }

    // Deletes the student and redirects back to the student list.
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await _repo.DeleteStudent(id))
        {
            _logger.LogError(
                "[StudentController] Student deletion failed for StudentId {StudentId}",
                id);

            return BadRequest("Student could not be deleted.");
        }

        return RedirectToAction(nameof(Index));
    }
}