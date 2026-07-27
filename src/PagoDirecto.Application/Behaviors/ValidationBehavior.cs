using FluentValidation;
using MediatR;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var result = new TResponse();
                result.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = ResponseMessage.ValidationError.GetString(),
                    NotificationTypeId = NotificationType.Warning
                };

                result.ValidationErrors = failures.Select(f => new PagoDirecto.Domain.Entities.ValidationError()
                {
                    Field = f.PropertyName,
                    Message = f.ErrorMessage
                }).ToList();

                return result;
            }
        }

        return await next();
    }
}
