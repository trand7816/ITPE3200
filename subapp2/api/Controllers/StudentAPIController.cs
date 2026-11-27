using System.Security.Claims;
using itpe3200.DAL;
using itpe3200.DTOs;
using itpe3200.Models;
using Microsoft.AspNetCore.Mvc;

namespace itpe3200.Controllers;

// [ApiController] returns 400 with the validation errors automatically when a DTO is invalid.
[ApiController]
[Route("api/student")]
public class StudentAPIController : ControllerBase
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<StudentAPIController> _logger;

    public StudentAPIController(ICourseRepository repo, ILogger<StudentAPIController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // GET api/student – all students
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _repo.GetAllStudents();
        if (students == null)
        {
            _logger.LogError("[StudentAPIController] Student list could not be loaded");
            return StatusCode(500, "Could not load the students.");
        }

        return Ok(students.Select(s => new StudentDto { Id = s.Id, Name = s.Name }));
    }

    // GET api/student/5 – one student with all groups the student is in
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var student = await _repo.GetStudentById(id);
        if (student == null)
        {
            _logger.LogWarning("[StudentAPIController] Student not found for StudentId {StudentId:0000}", id);
            return NotFound("Student not found.");
        }

        return Ok(new StudentDetailsDto
        {
            Id = student.Id,
            Name = student.Name,
            Groups = ToGroupDtos(student)
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogWarning("[StudentAPIController] Me called without a logged-in user");
            return Unauthorized("You must be logged in.");
        }

        var profile = await _repo.GetStudentByIdentityUserId(userId);
        if (profile == null)
        {
            _logger.LogWarning("[StudentAPIController] No student profile found for user {UserId}", userId);
            return NotFound("No student profile was found for your account.");
        }

        // GetStudentByIdentityUserId does not load the groups, so the student is loaded again with them
        var student = await _repo.GetStudentById(profile.Id);
        if (student == null)
        {
            _logger.LogError("[StudentAPIController] Could not load the groups for StudentId {StudentId:0000}", profile.Id);
            return StatusCode(500, "Could not load your groups.");
        }

        return Ok(ToGroupDtos(student));
    }

    // POST api/student – a student added by the teacher (not linked to a user account)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentDto dto)
    {
        var student = new Student { Name = dto.Name.Trim() };

        if (!await _repo.AddStudent(student))
        {
            _logger.LogError("[StudentAPIController] Student creation failed for {@student}", dto);
            return StatusCode(500, "Could not create the student.");
        }

        _logger.LogInformation("[StudentAPIController] Student {StudentName} created", student.Name);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, new StudentDto { Id = student.Id, Name = student.Name });
    }

    // PUT api/student/5 – changes the students name
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudentDto dto)
    {
        var student = await _repo.GetStudentById(id);
        if (student == null)
        {
            _logger.LogWarning("[StudentAPIController] Student not found when updating StudentId {StudentId:0000}", id);
            return NotFound("Student not found.");
        }

        student.Name = dto.Name.Trim();

        if (!await _repo.UpdateStudent(student))
        {
            _logger.LogError("[StudentAPIController] Student update failed for StudentId {StudentId:0000}", id);
            return StatusCode(500, "Could not update the student.");
        }

        _logger.LogInformation("[StudentAPIController] Student {StudentId:0000} updated", id);
        return Ok(new StudentDto { Id = student.Id, Name = student.Name });
    }

    // DELETE api/student/5 – the students group memberships are deleted with the student (cascade)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _repo.GetStudentById(id) == null)
        {
            _logger.LogWarning("[StudentAPIController] Student not found when deleting StudentId {StudentId:0000}", id);
            return NotFound("Student not found.");
        }

        if (!await _repo.DeleteStudent(id))
        {
            _logger.LogError("[StudentAPIController] Student deletion failed for StudentId {StudentId:0000}", id);
            return StatusCode(500, "Could not delete the student.");
        }

        _logger.LogInformation("[StudentAPIController] Student {StudentId:0000} deleted", id);
        return NoContent();
    }

    // The groups the student is in, with the session name (GetStudentById loads both)
    private static List<GroupDto> ToGroupDtos(Student student) => student.Groups
        .Where(m => m.Group != null)
        .Select(m => new GroupDto
        {
            Id = m.Group!.Id,
            Name = m.Group.Name,
            MaxSize = m.Group.MaxSize,
            CourseSessionId = m.Group.CourseSessionId,
            CourseSessionName = m.Group.CourseSession?.Name,
            MemberCount = m.Group.Members.Count
        })
        .ToList();
}
