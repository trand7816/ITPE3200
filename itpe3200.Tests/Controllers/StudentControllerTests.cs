using itpe3200.Controllers;
using itpe3200.DAL;
using itpe3200.Models;
using itpe3200.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace itpe3200.Tests.Controllers;

public class StudentControllerTests
{
    // Positive test:
    [Fact]
    public async Task Details_WhenStudentExists_ReturnViewResultWithStudents()
    {
        // Arrange 
        var student = new Student { Id = 1, Name = "Kari Nordmann" };

        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetStudentById(1).Returns(student);

        var mockLogger = Substitute.For<ILogger<StudentController>>();
        var studentController = new StudentController(mockRepository, mockLogger, null!);

        // Act
        var result = await studentController.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsAssignableFrom<StudentDetailsViewModel>(viewResult.ViewData.Model);
        Assert.Equal(student.Name, viewModel.Name);
    }

    // Negative test: Details returns NotFound when the student does not exist
    [Fact]
    public async Task Details_WhenStudentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetStudentById(1).Returns((Student?)null);

        var mockLogger = Substitute.For<ILogger<StudentController>>();
        var studentController = new StudentController(mockRepository, mockLogger, null!);

        // Act
        var result = await studentController.Details(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Positive test: Update redirects to the student's Details page when the update is saved
    [Fact]
    public async Task Update_WhenStudentIsValid_RedirectsToDetails()
    {
        // Arrange
        var model = new StudentUpdateViewModel { Id = 1, Name = "Kari Hansen" };
        var student = new Student { Id = 1, Name = "Kari Nordmann" };

        var mockRepository = Substitute.For<ICourseRepository>();
        mockRepository.GetStudentById(1).Returns(student);
        mockRepository.UpdateStudent(Arg.Any<Student>()).Returns(true);

        var mockLogger = Substitute.For<ILogger<StudentController>>();
        var studentController = new StudentController(mockRepository, mockLogger, null!);

        // Act
        var result = await studentController.Update(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(1, redirectResult.RouteValues!["id"]);

    }

    // Negative test: Update returns the form view with the model when the form is invalid
    [Fact]
    public async Task Update_WhenModelStateIsInvalid_ReturnsViewResultWithModel()
    {
        // Arrange
        var model = new StudentUpdateViewModel { Id = 1, Name = "" };

        var mockRepository = Substitute.For<ICourseRepository>();
        var mockLogger = Substitute.For<ILogger<StudentController>>();
        var studentController = new StudentController(mockRepository, mockLogger, null!);

        // Validation attributes do not run in unit tests, so we add the error ourselves
        studentController.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await studentController.Update(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsAssignableFrom<StudentUpdateViewModel>(viewResult.ViewData.Model);
        Assert.Equal(model, viewModel);

    }


}