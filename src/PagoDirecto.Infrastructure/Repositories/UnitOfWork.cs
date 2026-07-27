using PagoDirecto.Application.Extensions;
using PagoDirecto.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly Dictionary<string, object> _repositories = new();

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new GenericRepository<TEntity>(_context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<TEntity>)_repositories[type]!;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        // NO debemos hacer Dispose del _context manualmente aquí, 
        // ya que fue inyectado por DI y el framework se encarga de su ciclo de vida (Scoped).
        // Si lo destruimos nosotros, otros servicios en la misma petición fallarán con ObjectDisposedException.
        GC.SuppressFinalize(this);
    }
}

