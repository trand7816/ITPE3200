using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace itpe3200.Controllers;

public class SessionController : Controller
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<SessionController> _logger;

    public SessionController(ICourseRepository repo, ILogger<SessionController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sessions = await _repo.GetAllSessions();
        if (sessions is null)
        {
            _logger.LogError("[SessionController] Course sessions could not be loaded");
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
            _logger.LogWarning("[SessionController] Join attempted without a join code");
            ModelState.AddModelError(nameof(joinCode), "Join code is required.");
            return View();
        }

        var session = await _repo.GetSessionByJoinCode(joinCode.Trim().ToUpperInvariant());
        if (session is null)
        {
            _logger.LogWarning("[SessionController] No session found for JoinCode {JoinCode}", joinCode);
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
            _logger.LogWarning("[SessionController] Session creation failed validation {@model}", model);
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
            _logger.LogError("[SessionController] Session creation failed for session {@session}", session);
            return Problem("Unable to create the course session.");
        }

        _logger.LogInformation("[SessionController] Session {SessionName} created with JoinCode {JoinCode}", session.Name, session.JoinCode);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SessionCreateViewModel());
    }

    // Generates a random 6-character join code that is not already in use
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