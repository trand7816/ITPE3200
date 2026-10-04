using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace itpe3200.Controllers;

public class GroupController : Controller
{
    private readonly AppDbContext _appDbContext;
    //gir tilgang til database
    public GroupController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
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
    public IActionResult Create(GroupCreateViewModel model)
    {
        //Validerer at input fra skjemaet samsvarer med viewmodellen
        if (ModelState.IsValid)
        {
            //Validering basert på om course session ID finnes i databasen
            var courseSession = _appDbContext.CourseSessions.Find(model.CourseSessionId);
            if(courseSession == null)
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
                _appDbContext.Groups.Add(group);
                _appDbContext.SaveChanges();

                //forblir i create group-siden etter gruppen har blitt laget
                return RedirectToAction("Create");
            }
        }   
        //Henter course session-data til dropdown-menyen på nytt hvis forrige innsending feilet
        foreach(var courseSession in _appDbContext.CourseSessions)
        {
            model.CourseSessions.Add(new CourseSessionViewModel
            {
                Id = courseSession.Id,
                Name = courseSession.Name

            });
        }
        
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
            if(courseSession == null)
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
                _appDbContext.Students.Add(student);
                _appDbContext.SaveChanges();

                return RedirectToAction("Create");
            }
        }
        return View(model);
    }

    
}

