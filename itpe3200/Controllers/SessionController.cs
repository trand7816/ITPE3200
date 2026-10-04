using itpe3200.DAL;
using Microsoft.AspNetCore.Mvc;

namespace itpe3200.Controllers;

public class SessionController : Controller
{
    private readonly ICourseRepository _repo;

    public SessionController(ICourseRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Index()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public IActionResult Join()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult Join(string joinCode)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult Create(string name)
    {
        throw new NotImplementedException();
    }
}