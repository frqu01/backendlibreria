using MediatR;
using PagoDirecto.Domain.Entities;

namespace PagoDirecto.Application.CQRS;

public interface IQueryHandler<in TQuery> : IRequestHandler<TQuery, Result>
    where TQuery : IQuery
{
}
