using itpe3200.DAL;
using Microsoft.AspNetCore.Mvc;

namespace itpe3200.Controllers;

public class StudentController : Controller
{
    private readonly ICourseRepository _repo;

    public StudentController(ICourseRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public IActionResult Create(int sessionId, string name)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult AssignToGroup(int studentId, int? groupId)
    {
        throw new NotImplementedException();
    }
}