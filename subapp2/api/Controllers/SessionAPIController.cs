using itpe3200.DAL;
using itpe3200.DTOs;
using itpe3200.Models;
using Microsoft.AspNetCore.Mvc;

namespace itpe3200.Controllers;

// ApiController returns 400 with the validation errors automatically when a DTO is invalid
[ApiController]
[Route("api/session")]
public class SessionAPIController : ControllerBase
{
    private readonly ICourseRepository _repo;
    private readonly ILogger<SessionAPIController> _logger;

    public SessionAPIController(ICourseRepository repo, ILogger<SessionAPIController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // GET api/session – all sessions
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sessions = await _repo.GetAllSessions();
        if (sessions == null)
        {
            _logger.LogError("[SessionAPIController] Session list could not be loaded");
            return StatusCode(500, "Could not load the course sessions.");
        }

        return Ok(sessions.Select(ToSessionDto));
    }

    // GET api/session/5 – one session with its groups and members
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var session = await _repo.GetSessionById(id);
        if (session == null)
        {
            _logger.LogWarning("[SessionAPIController] Session not found for SessionId {SessionId:0000}", id);
            return NotFound("Course session not found.");
        }

        return Ok(ToSessionDetailsDto(session));
    }

    // POST api/session – the server makes the join code and the empty groups
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SessionDto dto)
    {
        var session = new CourseSession
        {
            Name = dto.Name.Trim(),
            JoinCode = await CreateUniqueJoinCode(),
            MaxGroups = dto.MaxGroups,
            RandomAssignment = dto.RandomAssignment
        };


        session.AddEmptyGroups(dto.MaxGroups, dto.GroupSize);

        if (!await _repo.CreateSession(session))
        {
            _logger.LogError("[SessionAPIController] Session creation failed for session {@session}", dto);
            return StatusCode(500, "Could not create the course session.");
        }

        _logger.LogInformation("[SessionAPIController] Session {SessionName} created with JoinCode {JoinCode}", session.Name, session.JoinCode);

        // GroupSize is not stored on the session, so it is sent back from the request
        var created = ToSessionDto(session);
        created.GroupSize = dto.GroupSize;
        return CreatedAtAction(nameof(GetById), new { id = session.Id }, created);
    }

    // PUT api/session/5 – changes name, number of groups and random assignment
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SessionDto dto)
    {
        var session = await _repo.GetSessionById(id);
        if (session == null)
        {
            _logger.LogWarning("[SessionAPIController] Session not found when updating SessionId {SessionId:0000}", id);
            return NotFound("Course session not found.");
        }

        session.Name = dto.Name.Trim();
        session.MaxGroups = dto.MaxGroups;
        session.RandomAssignment = dto.RandomAssignment;

        if (!await _repo.UpdateSession(session))
        {
            _logger.LogError("[SessionAPIController] Session update failed for SessionId {SessionId:0000}", id);
            return StatusCode(500, "Could not update the course session.");
        }

        _logger.LogInformation("[SessionAPIController] Session {SessionId:0000} updated", id);
        return Ok(ToSessionDto(session));
    }

    // DELETE api/session/5 – the groups and their members are deleted with the session (cascade)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _repo.GetSessionById(id) == null)
        {
            _logger.LogWarning("[SessionAPIController] Session not found when deleting SessionId {SessionId:0000}", id);
            return NotFound("Course session not found.");
        }

        if (!await _repo.DeleteSession(id))
        {
            _logger.LogError("[SessionAPIController] Session deletion failed for SessionId {SessionId:0000}", id);
            return StatusCode(500, "Could not delete the course session.");
        }

        _logger.LogInformation("[SessionAPIController] Session {SessionId:0000} deleted", id);
        return NoContent();
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

    // ---------------- Mapping from models to DTOs ----------------

    private static SessionDto ToSessionDto(CourseSession session) => new SessionDto
    {
        Id = session.Id,
        Name = session.Name,
        JoinCode = session.JoinCode,
        MaxGroups = session.MaxGroups,
        RandomAssignment = session.RandomAssignment
    };

    private static SessionDetailsDto ToSessionDetailsDto(CourseSession session) => new SessionDetailsDto
    {
        Id = session.Id,
        Name = session.Name,
        JoinCode = session.JoinCode,
        MaxGroups = session.MaxGroups,
        RandomAssignment = session.RandomAssignment,
        Groups = session.Groups.Select(g => new GroupDetailsDto
        {
            Id = g.Id,
            Name = g.Name,
            MaxSize = g.MaxSize,
            CourseSessionId = session.Id,
            CourseSessionName = session.Name,
            MemberCount = g.Members.Count,
            Members = g.Members.Select(m => new MemberDto
            {
                StudentId = m.StudentId,
                Name = m.Student?.Name ?? "",
                JoinedAt = m.JoinedAt
            }).ToList()
        }).ToList()
    };
}
