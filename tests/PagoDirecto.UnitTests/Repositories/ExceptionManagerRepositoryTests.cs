using PagoDirecto.Infrastructure.Repositories;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using FluentAssertions;
using Xunit;

namespace PagoDirecto.UnitTests.Repositories;

public class ExceptionManagerRepositoryTests
{
    private readonly Mock<ILogger<ExceptionManagerRepository>> _mockLogger;
    private readonly ExceptionManagerRepository _exceptionManager;

    public ExceptionManagerRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<ExceptionManagerRepository>>();
        _exceptionManager = new ExceptionManagerRepository(_mockLogger.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task HandlerExceptionApplication_ShouldSet500_AndGenerateReferenceId()
    {
        // Arrange
        var context = new Microsoft.AspNetCore.Http.DefaultHttpContext();
        var featureMock = new Mock<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        featureMock.Setup(f => f.Error).Returns(new Exception("Internal database failure"));
        
        context.Features.Set<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>(featureMock.Object);
        context.Response.Body = new System.IO.MemoryStream();

        // Act
        await _exceptionManager.HandlerExceptionApplication(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Be("application/json");
        
        context.Response.Body.Seek(0, System.IO.SeekOrigin.Begin);
        using var reader = new System.IO.StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        responseBody.Should().Contain("Ocurrió un error inesperado al procesar la solicitud.");
        responseBody.Should().Contain("Reference ID:");
        responseBody.Should().NotContain("Internal database failure"); // Ensure stack trace / error message is hidden
    }
}
