using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace itpe3200.Controllers;

public class GroupController : Controller
{
    private readonly AppDbContext _appDbContext;

    public GroupController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {

        var model = new GroupCreateViewModel();

        //henter kursøktene fra databasen og legger det til i listen 'CourseSession' i viewmodellen 'GroupCreateViewModel'.
        foreach (var courseSession in _appDbContext.CourseSessions)
        {
            model.CourseSessions.Add(new CourseSessionViewModel
            {
                Id = courseSession.Id,
                Name = courseSession.Name
            });

        }
    //sender objektet til Create.cshtml slik at det kan vises i en dropdown-meny
    return View(model);
    }

}