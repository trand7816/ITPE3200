using itpe3200.Models;

namespace itpe3200.Tests.Models;

public class CourseSessionTests
{
    // Positive test: AddEmptyGroups adds the given number of groups with the given max size
    [Fact]
    public void AddEmptyGroups_WhenValuesAreValid_AddsGroups ()
    {
        // Arrange: creates an empty session
        var session = new CourseSession { Name = "ITPE3200", JoinCode = "ABC123" };

        // Act: calls the method - asks for 3 groups with max size of 4 members 
        session.AddEmptyGroups(3,4);

        // Assert
        // The method changes the session instead of returning a result, so we check session.Groups
        Assert.Equal(3, session.Groups.Count);
        Assert.All(session.Groups, group => Assert.Equal(4, group.MaxSize));

    }

    // Negative test: AddEmptyGroups throws an exception when the number of groups is 0
    [Fact]
    public void AddEmptyGroups_WhenCountIsZero_ThrowsException ()
    {
        // Arrange
        var session = new CourseSession { Name = "ITPE3200", JoinCode = "ABC123" };

        // Act and Assert
        // The exception would crash the test if we called the method directly,
        // so Assert.Throws runs it and checks that the expected exception is thrown
        Assert.Throws<ArgumentOutOfRangeException>(() => session.AddEmptyGroups(0, 4));
    }

}