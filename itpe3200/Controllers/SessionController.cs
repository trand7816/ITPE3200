using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
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
    public async Task<IActionResult> Index()
    {
        var sessions = await _repo.GetAllSessions();
        if (sessions is null)
        {
            return Problem("Unable to load course sessions.");
        }

        return View(sessions);
    }

    [HttpGet]
    public IActionResult Join()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Join(string joinCode)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
        {
            ModelState.AddModelError(nameof(joinCode), "Join code is required.");
            return View();
        }

        var session = await _repo.GetSessionByJoinCode(joinCode.Trim().ToUpperInvariant());
        if (session is null)
        {
            ModelState.AddModelError(nameof(joinCode), "No course session was found for that join code.");
            return View();
        }

        return RedirectToAction("Join", "Group", new { joinCode = session.JoinCode });
    }

    [HttpPost]
    public async Task<IActionResult> Create(SessionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var joinCode = await CreateUniqueJoinCode();
        var session = new CourseSession
        {
            Name = model.Name.Trim(),
            JoinCode = joinCode,
            MaxGroups = model.MaxGroups,
            RandomAssignment = model.RandomAssignment
        };

        if (!await _repo.CreateSession(session))
        {
            return Problem("Unable to create the course session.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SessionCreateViewModel());
    }

    private async Task<string> CreateUniqueJoinCode()
    {
        const string characters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        while (true)
        {
            var code = string.Concat(
                Enumerable.Range(0, 6)
                    .Select(_ => characters[Random.Shared.Next(characters.Length)]));

            if (await _repo.GetSessionByJoinCode(code) is null)
            {
                return code;
            }
        }
    }
}