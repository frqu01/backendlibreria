using PagoDirecto.Domain.Entities;
using System.Data.Common;

namespace PagoDirecto.Application.Interfaces
{
    public interface IDataBaseContext
    {
        DbCommand Connection<D>();
        Task<Result> Create<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand);
        Task<Result> Read<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand);
        Task<Result> Update<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand);
        Task<Result> Delete<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand);
        Task<Result> Activate<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand);
    }
}

