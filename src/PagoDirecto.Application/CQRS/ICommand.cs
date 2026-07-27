using MediatR;
using PagoDirecto.Domain.Entities;

namespace PagoDirecto.Application.CQRS;

public interface ICommand : IRequest<Result>
{
}
