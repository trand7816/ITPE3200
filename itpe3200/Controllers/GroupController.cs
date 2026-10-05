using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace itpe3200.Controllers;

public class GroupController : Controller
{
    private readonly AppDbContext _appDbContext;
    private readonly ICourseRepository _repo;
    private readonly ILogger<GroupController> _logger;

    //gir tilgang til database, repository og logging
    public GroupController(AppDbContext appDbContext, ICourseRepository repo, ILogger<GroupController> logger)
    {
        _appDbContext = appDbContext;
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Create()
    {

        var model = new GroupCreateViewModel();

        //henter kursøktene fra databasen og legger det til i listen 'CourseSessions' i viewmodellen 'GroupCreateViewModel'.
        foreach (var courseSession in _appDbContext.CourseSessions)
        {
            model.CourseSessions.Add(new CourseSessionViewModel
            {
                Id = courseSession.Id,
                Name = courseSession.Name
            });

        }
        //sender objektet til Create.cshtml slik at groupsessions kan vises i en dropdown-meny
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GroupCreateViewModel model)
    {
        //Validerer at input fra skjemaet samsvarer med viewmodellen
        if (ModelState.IsValid)
        {
            //Validering basert på om course session ID finnes i databasen
            var courseSession = _appDbContext.CourseSessions.Find(model.CourseSessionId);
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
                    CourseSessionId = model.CourseSessionId
                };
                try
                {
                    _appDbContext.Groups.Add(group);
                    await _appDbContext.SaveChangesAsync();

                    //Hvis elev oppretter gruppe uten navn registreres gruppen som gruppe + id i databasen
                    if (string.IsNullOrWhiteSpace(group.Name))
                    {
                        group.Name = $"Group {group.Id}";
                        await _appDbContext.SaveChangesAsync();
                    }

                    _logger.LogInformation("[GroupController] Group {GroupName} created", group.Name);

                    //forblir i create group-siden etter gruppen har blitt laget
                    return RedirectToAction("Create");
                }
                catch (Exception e)
                {
                    _logger.LogError("[GroupController] Group creation failed for group {@group}, error message: {e}", group, e.Message);
                    ModelState.AddModelError("", "Could not save the group. Please try again.");
                }

            }
        }
        _logger.LogWarning("[GroupController] Group creation failed {@model}", model);

        //Henter course session-data til dropdown-menyen på nytt hvis forrige innsending feilet
        foreach (var courseSession in _appDbContext.CourseSessions)
        {
            model.CourseSessions.Add(new CourseSessionViewModel
            {
                Id = courseSession.Id,
                Name = courseSession.Name

            });
        }

        return View(model);
    }

    // GET: Group/Update/5 – shows the form filled in with the group's current values
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var group = await _appDbContext.Groups.FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            _logger.LogError("[GroupController] Group not found when updating GroupId {GroupId:0000}", id);
            return NotFound("Group not found");
        }
        return View(group);
    }

    // POST: Group/Update/5 – validates and saves the changes
    [HttpPost]
    public async Task<IActionResult> Update(Group model)
    {
        if (ModelState.IsValid)
        {
            var group = await _appDbContext.Groups.FirstOrDefaultAsync(g => g.Id == model.Id);

            if (group == null)
            {
                _logger.LogError("[GroupController] Group not found when updating GroupId {GroupId:0000}", model.Id);
                return NotFound("Group not found");
            }

            group.Name = model.Name;
            group.MaxSize = model.MaxSize;

            try
            {
                await _appDbContext.SaveChangesAsync();
                _logger.LogInformation("[GroupController] Group {GroupId:0000} updated", group.Id);
                return RedirectToAction("Table");
            }
            catch (Exception e)
            {
                _logger.LogError("[GroupController] Group update failed for group {@group}, error message: {e}", group, e.Message);
                ModelState.AddModelError("", "Could not update group. Please try again");
            }
        }
        _logger.LogWarning("[GroupController] Group update failed {@model}", model);
        return View(model);
    }

    [HttpGet]
    public IActionResult Join()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Join(JoinViewModel model)
    {
        if (ModelState.IsValid)
        {
            //Sjekker om input-koden er lik JoinCode i databasen, returnerer null dersom den ikke finnes i db
            var courseSession = _appDbContext.CourseSessions.FirstOrDefault(c => c.JoinCode == model.JoinCode);
            if (courseSession == null)
            {
                ModelState.AddModelError("JoinCode", "Invalid join code");
            }
            else
            {
                var student = new Student
                {
                    Name = model.Name,
                    CourseSessionId = courseSession.Id
                };
                try
                {
                    _appDbContext.Students.Add(student);
                    _appDbContext.SaveChanges();
                    _logger.LogInformation("[GroupController] Student {StudentName} joined session {SessionId:0000}", student.Name, courseSession.Id);

                    return RedirectToAction("Create");
                }
                catch (Exception e)
                {
                    _logger.LogError("[GroupController] Student join failed for student {@student}, error message: {e}", student, e.Message);
                    ModelState.AddModelError("", "Could not join the session. Please try again.");
                }
            }
        }
        _logger.LogWarning("[GroupController] Join failed for JoinCode {JoinCode}", model.JoinCode);
        return View(model);
    }

    // GET: Group/Delete/5 – shows a confirmation page before deleting
    [HttpGet]
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