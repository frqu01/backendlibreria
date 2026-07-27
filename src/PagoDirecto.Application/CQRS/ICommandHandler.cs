using MediatR;
using PagoDirecto.Domain.Entities;

namespace PagoDirecto.Application.CQRS;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}
