using PagoDirecto.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PagoDirecto.UnitTests.Entities;

public class ResultTests
{
    [Fact]
    public void IsSuccessful_ShouldReturnFalse_WhenRequestStatusIsNull()
    {
        // Arrange
        var resultObj = new Result
        {
            RequestStatus = null
        };

        // Act
        var isSuccess = resultObj.IsSuccessful();

        // Assert
        isSuccess.Should().BeFalse();
    }

    [Fact]
    public void IsSuccessful_ShouldReturnTrue_WhenRequestStatusIsSuccessIsTrue()
    {
        // Arrange
        var resultObj = new Result
        {
            RequestStatus = new RequestStatus
            {
                IsSuccess = true
            }
        };

        // Act
        var isSuccess = resultObj.IsSuccessful();

        // Assert
        isSuccess.Should().BeTrue();
    }

    [Fact]
    public void IsSuccessful_ShouldReturnFalse_WhenRequestStatusIsSuccessIsFalse()
    {
        // Arrange
        var resultObj = new Result
        {
            RequestStatus = new RequestStatus
            {
                IsSuccess = false
            }
        };

        // Act
        var isSuccess = resultObj.IsSuccessful();

        // Assert
        isSuccess.Should().BeFalse();
    }
}

