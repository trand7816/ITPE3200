using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using itpe3200.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace itpe3200.Controllers;

// Each action has a role in addition to authorize on class level.
// Teachers manage groups (create, edit, delete, overview),
// students join groups, and both can see group details.
[Authorize]
public class GroupController : Controller
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<GroupController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    // Gives access to the repository, logging and the logged-in user. All database access goes through the repository
    public GroupController(ICourseRepository repo, ILogger<GroupController> logger, UserManager<IdentityUser> userManager)
    {
        _repo = repo;
        _logger = logger;
        _userManager = userManager;

    }

    [HttpGet]
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
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

            if (model.MaxSize < memberCount)
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
    [Authorize(Roles = "Student")]
    public IActionResult Join()
    {
        return View();
    }

    //Method to join session
    [HttpPost]
    [Authorize(Roles = "Student")]
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
                if (session == null)
                {
                    return NotFound();
                }

                //Random assignment: places a student in a group right away and skips student choosing a group
                if (session.RandomAssignment)
                {
                    var student = await GetLoggedInStudent();
                    if (student == null)
                    {
                        ModelState.AddModelError("JoinCode", "No student profile was found for your account.");
                        return View(model);
                    }

                    var (result, groupId) = await _repo.AssignStudentRandomly(student.Id, session.Id);
                    if (result == GroupJoinResult.Success && groupId.HasValue)
                    {
                        _logger.LogInformation("[GroupController] Student {studentId} randomly assigned to group {groupId}", student.Id, groupId);
                        return RedirectToAction(nameof(Joined), new { id = groupId.Value });
                    }
                    _logger.LogWarning("[GroupController] Random assignment failed for student {StudentId}:{Result}", student.Id, result);
                    ModelState.AddModelError("JoinCode", JoinErrorMessage(result));
                    return View(model);

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
    //Method for students to join group 
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> JoinGroup(int groupId, JoinViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid information when joining {GroupId}", groupId);

            return View("Join", model);
        }

        var session = await _repo.GetSessionByJoinCode(model.JoinCode);
        var group = await _repo.GetGroupById(groupId);

        if (session == null)
        {
            _logger.LogWarning("Invalid join code provided");

            ModelState.AddModelError("", "Invalid join code.");

            return View("Join", model);
        }

        if (group == null || group.CourseSessionId != session.Id)
        {
            _logger.LogWarning("Invalid group {GroupId} for session {SessionId}", groupId, session.Id);

            ModelState.AddModelError("", "Invalid group selected.");
            return View("Join", model);
        }

        //Find the student profile for the logged in user
        var student = await GetLoggedInStudent();
        if (student == null)
        {
            _logger.LogWarning("[GroupController] No student profile found for the logged-in user");
            ModelState.AddModelError("", "No student profile was found for your account.");
            return View("Join", model);
        }

        //Try to assign student to the specified group
        var result = await _repo.AssignStudentToGroup(student.Id, group.Id);
        if (result == GroupJoinResult.Success)
        {
            _logger.LogInformation("Student {StudentId} joined group {GroupId}", student.Id, groupId);
            return RedirectToAction(nameof(Joined), new { id = groupId });
        }

        ModelState.AddModelError("", JoinErrorMessage(result));

        //Reload groups to be displayed again for the Join page
        model.CourseSessionId = session.Id;
        model.CourseSessionName = session.Name;
        model.Groups = (await _repo.GetSessionById(session.Id))?.Groups.ToList();

        return View("Join", model);
    }

    //GET: Group/Joined/5 - confirmation page after a student has joined a group (step 3. of join flow)
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Joined(int id)
    {
        var group = await _repo.GetGroupById(id);
        if (group is null)
        {
            _logger.LogWarning("[GroupController] Group not found for GroupId {GroupId:0000} on the joined page", id);
            return NotFound("Group not found");
        }

        return View(group);
    }

    //GET: Group/Leave/5 - asks the student to confirm before leaving the group
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Leave(int id)
    {
        var group = await _repo.GetGroupById(id);
        if (group is null)
        {
            _logger.LogWarning("[GroupController] Group not found for GroupId {GroupId:0000} on the leave page", id);
            return NotFound("Group not found");
        }
        return View(group);
    }

    //POST: Group/Leave - a student leaves a group they are a member of
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> LeaveConfirmed(int groupId)
    {
        var student = await GetLoggedInStudent();
        if (student is null)
        {
            _logger.LogWarning("[GroupController] No student profile found when leaving group {GroupId:0000}", groupId);
            return Unauthorized();
        }

        //Returns false if student is not a member of the group
        if (!await _repo.RemoveStudentFromGroup(student.Id, groupId))
        {
            _logger.LogWarning("[GroupController] Student {StudentId} could not leave group {GroupId:0000}", student.Id, groupId);
            return BadRequest("Could not leave the group");
        }
        _logger.LogInformation("[GroupController] Student {StudentId} left group {GroupId:0000}", student.Id, groupId);
        return RedirectToAction("Student", "Home", null, "my-groups");
    }

    //POST: Group/RemoveMember - the teacher removes a student from a group  
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> RemoveMember(int groupId, int studentId)
    {
        //Returns false if student is not a member of the group
        if (!await _repo.RemoveStudentFromGroup(studentId, groupId))
        {
            _logger.LogWarning("[GroupController] Student {StudentId} could not be removed from group {GroupId:0000}", studentId, groupId);
            return BadRequest("Could not be removed from group");
        }
        _logger.LogInformation("[GroupController] Student {StudentId} was removed from group {GroupId:0000} by a teacher", studentId, groupId);
        return RedirectToAction(nameof(Details), new { id = groupId});
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> AssignStudent(int studentId, int groupId)
    {
        var result = await _repo.AssignStudentToGroup(studentId, groupId);

        if (result == GroupJoinResult.Success)
        {
            _logger.LogInformation("Student {StudentId} assigned to group {GroupId}", studentId, groupId);
            return RedirectToAction("Details", new { id = groupId });
        }

        _logger.LogWarning("Could not assign student {StudentId} to group {GroupId}: {Result}", studentId, groupId, result);

        //Switch case for every case where result did not return Success
        ModelState.AddModelError("", result switch
        {
            GroupJoinResult.GroupFull => "The group is full.",
            GroupJoinResult.AlreadyInSession => "Student already belongs to a group in this session.",
            GroupJoinResult.StudentNotFound => "Student not found.",
            GroupJoinResult.GroupNotFound => "Group not found.",
            _ => "Could not assign student."
        });

        var group = await _repo.GetGroupById(groupId);
        if (group == null)
        {
            return NotFound();
        }

        var students = await _repo.GetAllStudents();
        if (students == null)
        {
            return Problem("Could not load students");
        }

        var viewModel = new GroupDetailsViewModel { Group = group, Students = students };
        return View("Details", viewModel);
    }


    // GET: Group/Delete/5 – shows a confirmation page before deleting
    [HttpGet]
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
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
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Table(string? search, int? sessionId)
    {
        var model = await BuildFilteredGroupOverview(search, sessionId);
        if (model is null)
        {
            return Problem("Could not load groups.");
        }
        return View(model);
    }

    // GET: Group/Details/5 – shows details for one group
    [HttpGet]
    // Adding both roles ensures that if a user slips through and does not get assigned a role, 
    // they will still not be able to see details
    [Authorize(Roles = "Teacher,Student")]
    public async Task<IActionResult> Details(int id)
    {
        var group = await _repo.GetGroupById(id);

        if (group is null)
        {
            _logger.LogError("[GroupController] Group not found for GroupId {GroupId:0000}", id);
            return NotFound("Group not found");
        }
        var viewModel = new GroupDetailsViewModel { Group = group };

        // Only teachers need the list of all students - for the "Assign student" form
        if (User.IsInRole("Teacher"))
        {
            var students = await _repo.GetAllStudents();
            if (students == null)
            {
                return Problem("Could not load students");
            }

            viewModel.Students = students;
        }

        return View(viewModel);
    }



    // GET: Group/Grid – shows a grid of all groups
    [HttpGet]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Grid(string? search, int? sessionId)
    {
        var model = await BuildFilteredGroupOverview(search, sessionId);
        if (model is null)
        {
            return Problem("Could not load groups.");
        }
        return View(model);
    }

    //Finds student profile linked to user, or returns null if there is none.
    //Used by both join actions (choose group, and random assignment).
    private async Task<Student?> GetLoggedInStudent()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return null;
        }
        return await _repo.GetStudentByIdentityUserId(user.Id);
    }

    //Different error messages if a join attempt fails
    private static string JoinErrorMessage(GroupJoinResult result) => result switch
    {
        GroupJoinResult.GroupFull => "The group is full.",
        GroupJoinResult.SessionFull => "All groups in this session is full, ask your teacher for help.",
        GroupJoinResult.AlreadyInSession => "You already belong to a group in this course session.",
        GroupJoinResult.StudentNotFound => "Student not found.",
        GroupJoinResult.GroupNotFound => "Group not found.",
        _ => "Could not join the group."
    };

    //Used by Table and Grid view: returns groups matching the search text and/or session
    private async Task<GroupsViewModel?> BuildFilteredGroupOverview(string? search, int? sessionId)
    {
        var groups = await _repo.GetAllGroups();

        if (groups is null)
        {
            _logger.LogError("[GroupController] Could not load groups for group overview.");
            return null;
        }

        var sessions = await _repo.GetAllSessions();

        if (sessions is null)
        {
            _logger.LogError("[GroupController] Could not load course sessions for group filters");
            return null;
        }

        var filteredGroups = groups.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();

            filteredGroups = filteredGroups.Where(group =>
                (group.Name?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)
                || (group.CourseSession?.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (sessionId.HasValue)
        {
            filteredGroups = filteredGroups
                .Where(group => group.CourseSessionId == sessionId.Value);
        }

        return new GroupsViewModel
        {
            Groups = filteredGroups.ToList(),
            Search = search?.Trim() ?? "",
            SessionId = sessionId,
            CourseSessions = sessions
                .Select(session => new CourseSessionViewModel
                {
                    Id = session.Id,
                    Name = session.Name
                })
                .ToList()
        };

    }
}