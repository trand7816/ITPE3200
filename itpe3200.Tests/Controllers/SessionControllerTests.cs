using itpe3200.Controllers;
using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace itpe3200.Tests.Controllers;

public class SessionControllerTests
{
    // Positive test: Create redirects to Index when the session is valid and saved
    [Fact]
    public async Task Create_WhenSessionIsValid_RedirectsToIndex()
    {
        // Arrange
        var model = new SessionCreateViewModel { Name = "ITPE3200", MaxGroups = 3, GroupSize = 4 };

        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetSessionByJoinCode(Arg.Any<string>()).Returns((CourseSession?)null);
        mockRepository.CreateSession(Arg.Any<CourseSession>()).Returns(true);

        var mockLogger = Substitute.For<ILogger<SessionController>>();
        var sessionController = new SessionController(mockRepository, mockLogger);

        // Act
        var result = await sessionController.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    // Negative test: Create returns Problem when the session could not be saved
    [Fact]
    public async Task Create_WhenSavingFails_ReturnsProblem()
    {
        // Arrange
        var model = new SessionCreateViewModel { Name = "ITPE3200", MaxGroups = 3, GroupSize = 4 };

        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetSessionByJoinCode(Arg.Any<string>()).Returns((CourseSession?)null);
        mockRepository.CreateSession(Arg.Any<CourseSession>()).Returns(false);

        var mockLogger = Substitute.For<ILogger<SessionController>>();
        var sessionController = new SessionController(mockRepository, mockLogger);


        // Act
        var result = await sessionController.Create(model);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problemResult.StatusCode);
    }
}
