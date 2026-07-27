using PagoDirecto.Domain.Enums;
using PagoDirecto.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace PagoDirecto.UnitTests.Repositories;

public class ResponseFactoryRepositoryTests
{
    private readonly ResponseFactoryRepository _factory;

    public ResponseFactoryRepositoryTests()
    {
        _factory = new ResponseFactoryRepository();
    }

    [Fact]
    public void Success_ShouldReturnResult_WithIsSuccessTrue_AndSuccessNotification()
    {
        // Act
        var result = _factory.Success("Operación completada", "Datos de prueba");

        // Assert
        result.RequestStatus.Should().NotBeNull();
        result.RequestStatus!.IsSuccess.Should().BeTrue();
        result.RequestStatus.NotificationType.Should().Be(NotificationType.Success);
        result.RequestStatus.ResponseMessage.Should().Be("Operación completada.");
        result.Data.Should().Be("Datos de prueba");
    }

    [Fact]
    public void Error_ShouldReturnResult_WithIsSuccessFalse_AndErrorNotification()
    {
        // Act
        var result = _factory.Error("Ocurrió un error");

        // Assert
        result.RequestStatus.Should().NotBeNull();
        result.RequestStatus!.IsSuccess.Should().BeFalse();
        result.RequestStatus.NotificationType.Should().Be(NotificationType.Error);
        result.RequestStatus.ResponseMessage.Should().Be("Ocurrió un error.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public void Warning_ShouldReturnResult_WithIsSuccessFalse_AndWarningNotification()
    {
        // Act
        var result = _factory.Warning("Aviso de prueba");

        // Assert
        result.RequestStatus.Should().NotBeNull();
        result.RequestStatus!.IsSuccess.Should().BeFalse();
        result.RequestStatus.NotificationType.Should().Be(NotificationType.Warning);
        result.RequestStatus.ResponseMessage.Should().Be("Aviso de prueba.");
    }
}
