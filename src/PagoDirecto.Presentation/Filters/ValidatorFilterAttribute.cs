using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace PagoDirecto.Presentation.Filters;

public class ValidatorFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errores = new List<ValidationError>();

            RequestStatus estadoSolicitudApi = new()
            {
                IsSuccess = false,
                ResponseMessage = ResponseMessage.ValidationError.GetDescription(),
                NotificationType = NotificationType.Warning
            };

            foreach (var modelStateKey in context.ModelState.Keys)
            {
                var value = context.ModelState[modelStateKey];
                if (value == null) continue;

                foreach (var error in value.Errors)
                {
                    ValidationError erroresValidacionApi = new()
                    {
                        Field = modelStateKey,
                        Message = error.ErrorMessage
                    };
                    errores.Add(erroresValidacionApi);
                }
            }

            var resultadoApi = new Result()
            {
                RequestStatus = estadoSolicitudApi,
                ValidationErrors = errores.Count > 0 ? errores : null
            };

            context.Result = new JsonResult(resultadoApi)
            {
                StatusCode = 400
            };
        }
    }
}

