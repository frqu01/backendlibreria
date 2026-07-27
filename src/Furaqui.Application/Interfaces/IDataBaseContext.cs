using Furaqui.Domain.Entities;
using System.Data.Common;

namespace Furaqui.Application.Interfaces
{
    public interface IDataBaseContext
    {
        DbCommand Connection<D>();
        Task<Result> Create<T>(string storeProcedureName, object? Parameters, DbCommand dbCommand);
        Task<Result> Read<T>(string storeProcedureName, object? Parameters, DbCommand dbCommand);
        Task<Result> Update<T>(string storeProcedureName, object? Parameters, DbCommand dbCommand);
        Task<Result> Delete<T>(string storeProcedureName, object? Parameters, DbCommand dbCommand);
        Task<Result> Activate<T>(string storeProcedureName, object? Parameters, DbCommand dbCommand);
    }
}
