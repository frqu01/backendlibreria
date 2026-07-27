using PagoDirecto.Presentation.Filters;
using PagoDirecto.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace PagoDirecto.UnitTests.Filters;

public class ValidatorFilterAttributeTests
{
    [Fact]
    public void OnActionExecuting_ShouldSetBadRequestResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var filter = new ValidatorFilterAttribute();
        var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
        modelState.AddModelError("Email", "El correo es requerido");

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            modelState
        );

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object()
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        context.Result.Should().NotBeNull();
        context.Result.Should().BeOfType<JsonResult>();
        
        var jsonResult = (JsonResult)context.Result!;
        jsonResult.StatusCode.Should().Be(400);

        var resultData = jsonResult.Value as Result;
        resultData.Should().NotBeNull();
        resultData!.RequestStatus!.IsSuccess.Should().BeFalse();
        resultData.ValidationErrors.Should().NotBeNullOrEmpty();
        resultData.ValidationErrors![0].Field.Should().Be("Email");
        resultData.ValidationErrors[0].Message.Should().Be("El correo es requerido");
    }

    [Fact]
    public void OnActionExecuting_ShouldNotSetResult_WhenModelStateIsValid()
    {
        // Arrange
        var filter = new ValidatorFilterAttribute();
        var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary(); // No errors added

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            modelState
        );

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            new object()
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeNull(); // It should let the request pass through
    }
}
