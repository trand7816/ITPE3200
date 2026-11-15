using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using itpe3200.Services;

namespace itpe3200.Controllers;

//[Authorize]
public class GroupController : Controller
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<GroupController> _logger;

    //gir tilgang til repository og logging. All databasetilgang går gjennom repositoryet
    public GroupController(ICourseRepository repo, ILogger<GroupController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var model = new GroupCreateViewModel();

        //henter kursøktene fra databasen og legger det til i listen 'CourseSessions' i viewmodellen 'GroupCreateViewModel'.
        if (!await LoadCourseSessions(model))
        {
            return Problem("Could not load course sessions.");
        }

        //sender objektet til Create.cshtml slik at groupsessions kan vises i en dropdown-meny
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(GroupCreateViewModel model)
    {
        //Validerer at input fra skjemaet samsvarer med viewmodellen
        if (ModelState.IsValid)
        {
            //Validering basert på om course session ID finnes i databasen
            var courseSession = await _repo.GetSessionById(model.CourseSessionId ?? 0);
            if (courseSession == null)
            {
                ModelState.AddModelError("CourseSessionId", "Invalid course session ID");
            }
            else
            {
                //lagrer ny gruppe i databasen
                var group = new Group
                {
                    Name = model.Name,
                    MaxSize = model.MaxSize,
                    // Use the session we found above so the id is never null
                    CourseSessionId = courseSession.Id
                };

                bool ok = await _repo.CreateGroup(group);

                //Hvis elev oppretter gruppe uten navn registreres gruppen som gruppe + id i databasen
                if (ok && string.IsNullOrWhiteSpace(group.Name))
                {
                    group.Name = $"Group {group.Id}";
                    ok = await _repo.UpdateGroup(group);
                }

                if (ok)
                {
                    _logger.LogInformation("[GroupController] Group {GroupName} created", group.Name);

                    // User gets sent to Table-view as confirmation
                    return RedirectToAction("Table");
                }

                // The repository has already logged the database error
                ModelState.AddModelError("", "Could not save the group. Please try again.");
            }
        }
        _logger.LogWarning("[GroupController] Group creation failed {@model}", model);

        //Henter course session-data til dropdown-menyen på nytt hvis forrige innsending feilet
        if (!await LoadCourseSessions(model))
        {
            return Problem("Could not load course sessions.");
        }

        return View(model);
    }

    // Fills the dropdown list in the Create form with all course sessions

    private async Task<bool> LoadCourseSessions(GroupCreateViewModel model)
    {
        var sessions = await _repo.GetAllSessions();
        if (sessions is null)
        {
            _logger.LogError("[GroupController] Could not load course sessions for the Create form");
            return false;
        }

        model.CourseSessions.Clear();
        foreach (var courseSession in sessions)
        {
            model.CourseSessions.Add(new CourseSessionViewModel
            {
                Id = courseSession.Id,
                Name = courseSession.Name
            });
        }
        return true;
    }

    // GET: Group/Update/5 – shows the form filled in with the group's current values
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Update(int id)
    {
        var group = await _repo.GetGroupById(id);

        if (group == null)
        {
            _logger.LogError("[GroupController] Group not found when updating GroupId {GroupId:0000}", id);
            return NotFound("Group not found");
        }
        
        var model = new GroupUpdateViewModel
        {
            Id = group.Id,
            Name = group.Name,
            MaxSize = group.MaxSize
        };
        return View(model);
    }

    // POST: Group/Update/5 – validates and saves the changes
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(GroupUpdateViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Load the saved group first, so fields that are not in the form

            var group = await _repo.GetGroupById(model.Id);

            if (group == null)
            {
                _logger.LogError("[GroupController] Group not found when updating GroupId {GroupId:0000}", model.Id);
                return NotFound("Group not found");
            }

            int memberCount = group.Members.Count;

            if(model.MaxSize < memberCount)
            {
                ModelState.AddModelError(nameof(model.MaxSize), $"Group already has {memberCount} members. " 
                + $"Maximum size cannot be less than {memberCount}");
                
                return View(model);
            }

            group.Name = model.Name;
            group.MaxSize = model.MaxSize;

            if (await _repo.UpdateGroup(group))
            {
                _logger.LogInformation("[GroupController] Group {GroupId:0000} updated", group.Id);
                return RedirectToAction("Table");
            }

            // The repository has already logged the database error
            ModelState.AddModelError("", "Could not update group. Please try again");
        }
        _logger.LogWarning("[GroupController] Group update failed {@model}", model);
        return View(model);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Join()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Join(JoinViewModel model)
    {
        if (ModelState.IsValid)
        {
            //Sjekker om input-koden er lik JoinCode i databasen, returnerer null dersom den ikke finnes i db
            var courseSession = await _repo.GetSessionByJoinCode(model.JoinCode);
            if (courseSession == null)
            {
                ModelState.AddModelError("JoinCode", "Invalid join code");
            }
            else
            {
                var session = await _repo.GetSessionById(courseSession.Id);
                if(session == null)
                {
                    return NotFound();
                }
                model.CourseSessionId = session.Id;
                model.CourseSessionName = session.Name;
                model.Groups = session.Groups.ToList();
                
                return View(model);
            } 
        }
        _logger.LogWarning("[GroupController] Join failed for JoinCode {JoinCode}", model.JoinCode);
        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> JoinGroup(int groupId, JoinViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid student information");
        }
        
        var session = await _repo.GetSessionByJoinCode(model.JoinCode);
        var group = await _repo.GetGroupById(groupId);

        if(session == null || group == null || group.CourseSessionId != session.Id)
        {
            return BadRequest("Invalid group or join code");
        }

        // TODO: Find the existing Student using the logged-in Identity user.
        // Then call _repo.AssignStudentToGroup(student.Id, groupId).
        ModelState.AddModelError("", "Group joining is temporarily unavailable until student login is connected.");
        model.CourseSessionId = session.Id;
        model.CourseSessionName = session.Name;
        model.Groups = (await _repo.GetSessionById(session.Id))?.Groups.ToList();

        return View("Join", model);
    }


    // GET: Group/Delete/5 – shows a confirmation page before deleting
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _repo.GetGroupById(id);
        if (group == null)
        {
            _logger.LogError("[GroupController] Group not found for GroupId {GroupId:0000}", id);
            return NotFound("Group not found");
        }
        return View(group);
    }

    // POST: Group/DeleteConfirmed/5 – performs the delete
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool ok = await _repo.DeleteGroup(id);
        if (!ok)
        {
            _logger.LogError("[GroupController] Group deletion failed for GroupId {GroupId:0000}", id);
            return BadRequest("Group deletion failed");
        }
        _logger.LogInformation("[GroupController] Group {GroupId:0000} deleted", id);
        return RedirectToAction("Table");
    }

    // GET: Group/Table – shows a table of all groups
    [HttpGet]
    public async Task<IActionResult> Table()
    {
        var groups = await _repo.GetAllGroups();

        if (groups is null)
        {
            _logger.LogError("[GroupController] Could not load groups for table");
            return Problem("Could not load groups.");
        }

        var model = new GroupsViewModel
        {
            Groups = groups
        };

        return View(model);
    }

    // GET: Group/Details/5 – shows details for one group
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var group = await _repo.GetGroupById(id);

        if (group is null)
        {
            _logger.LogError("[GroupController] Group not found for GroupId {GroupId:0000}", id);
            return NotFound("Group not found");
        }

        return View(group);
    }

    // GET: Group/Grid – shows a grid of all groups
    [HttpGet]
    public async Task<IActionResult> Grid()
    {
        var groups = await _repo.GetAllGroups();

        if (groups is null)
        {
            _logger.LogError("[GroupController] Could not load groups for grid");
            return Problem("Could not load groups.");
        }

        var model = new GroupsViewModel
        {
            Groups = groups
        };

        return View(model);
    }
}