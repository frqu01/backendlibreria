using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class DbConnectionHandler : IDisposable
    {
        private readonly DbCommand _dbCommand;
        public DbConnectionHandler(DbCommand dbCommand)
        {
            _dbCommand = dbCommand;
        }
        public DbCommand Open()
        {
            try
            {
                _dbCommand.Connection?.Open();
            }
            catch (SqlException)
            {
                if (_dbCommand != null && _dbCommand.Connection?.State != ConnectionState.Closed)
                {
                    _dbCommand.Connection?.Close();
                    _dbCommand.Dispose();
                }

                throw;
            }
            catch (Exception)
            {
                if (_dbCommand != null && _dbCommand.Connection?.State != ConnectionState.Closed)
                {
                    _dbCommand.Connection?.Close();
                    _dbCommand.Dispose();
                }

                throw;
            }

            return _dbCommand;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbCommand.Connection != null)
                {
                    _dbCommand.Connection.Close();
                    _dbCommand.Connection.Dispose();
                    _dbCommand.Connection = null;
                }
            }
        }
    }
}

