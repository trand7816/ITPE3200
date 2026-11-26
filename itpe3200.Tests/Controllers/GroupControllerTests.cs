using itpe3200.Controllers;
using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace itpe3200.Tests.Controllers;

public class GroupControllerTests
{
    // Positive test: Table returns a view with all groups when the repository has groups
    [Fact]
    public async Task Table_WhenGroupsExist_ReturnsViewResultWithGroups()
    {
        // Arrange mock data
        var groupList = new List<Group>
        {
            new Group { Id = 1, Name = "Group A", MaxSize = 4, CourseSessionId = 1 },
            new Group { Id = 2, Name = "Group B", MaxSize = 4, CourseSessionId = 1 }
        };

        var sessionList = new List<CourseSession>
        {
            new CourseSession { Id = 1, Name = "ITPE3200", JoinCode = "ABC123" }
        };

        // Arrange: substitutes instead of the real repository and logger
        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetAllGroups().Returns(groupList);
        mockRepository.GetAllSessions().Returns(sessionList);
        var mockLogger = Substitute.For<ILogger<GroupController>>();

        // UserManager is not used by Table, so null is passed
        var groupController = new GroupController(mockRepository, mockLogger, null!);

        // Act: no search text and no session filter
        var result = await groupController.Table(null, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var groupsViewModel = Assert.IsAssignableFrom<GroupsViewModel>(viewResult.ViewData.Model);
        Assert.Equal(2, groupsViewModel.Groups.Count());
        Assert.Equal(groupList, groupsViewModel.Groups);
    }

    // Negative test: Table returns a Problem result when groups can't be loaded
    [Fact]
    public async Task Table_WhenGroupsAreNull_ReturnsProblem()
    {
        // Arrange
        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetAllGroups().Returns((IEnumerable<Group>?)null);
        var mockLogger = Substitute.For<ILogger<GroupController>>();

        var groupController = new GroupController(mockRepository, mockLogger, null!);

        // Act
        var result = await groupController.Table(null, null);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problemResult.StatusCode);
    }

    // Positive test: Create redirects to Table when the form is valid and the group is saved
    [Fact]
    public async Task Create_WhenGroupIsValid_RedirectsToTable()
    {
        // Arrange
        var model = new GroupCreateViewModel
        {
            Name = "Group A",
            MaxSize = 4,
            CourseSessionId = 1
        };

        var courseSession = new CourseSession { Id = 1, Name = "ITPE3200", JoinCode = "ABC123" };

        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetSessionById(1).Returns(courseSession);
        mockRepository.CreateGroup(Arg.Any<Group>()).Returns(true);

        var mockLogger = Substitute.For<ILogger<GroupController>>();
        var groupController = new GroupController(mockRepository, mockLogger, null!);

        // Act
        var result = await groupController.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Table", redirectResult.ActionName);
    }
}