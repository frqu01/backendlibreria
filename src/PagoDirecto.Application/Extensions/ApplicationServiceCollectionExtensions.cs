using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PagoDirecto.Application.Behaviors;
using System.Reflection;

namespace PagoDirecto.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddPagoDirectoCQRS(this IServiceCollection services, Assembly consumerAssembly)
    {
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(consumerAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(consumerAssembly);

        return services;
    }
}
