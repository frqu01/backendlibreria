using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using SchemaType = PagoDirecto.Domain.Enums.SchemaType;
using PagoDirecto.Application.Interfaces;
using PagoDirecto.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class DataBaseContextRepository : IDataBaseContext
    {
        protected readonly ILogRecorder _iLoggerApi;
        protected readonly IExceptionFactory _iExceptionFactory;
        public DataBaseContextRepository(ILogRecorder iLoggerApi,
            IExceptionFactory iExceptionFactory,
            IAppConfiguration iAppConfiguration)
        {
            _iLoggerApi = iLoggerApi;
            _iExceptionFactory = iExceptionFactory;
        }
        
        private async Task<Result> StoreProcedure<T>(SchemaType tipoEsquemaApi, StoredProcedure procedimientoAlmacenadoApi)
        {
            //Validar
            if (procedimientoAlmacenadoApi == null)
            {
                throw _iExceptionFactory.Warning("No se envió los datos del procedimiento almacenado a Infraestructura.");
            }

            var listaParametersProcedimiento = ListaParameters(procedimientoAlmacenadoApi.Parameters);

            //Nombre del procedimiento almacenado
            string nombreProcedimientoCompleto = tipoEsquemaApi.GetString() + "." + procedimientoAlmacenadoApi.ProcedureName;

            //Validar entidad
            if (procedimientoAlmacenadoApi.Parameters == null)
            {
                throw _iExceptionFactory.Warning("Los parámetos no fueron enviados a Infraestructura.");
            }


            //Ejecutar procedimiento almacenado 
            var sqlParameters = ListaParameters(procedimientoAlmacenadoApi.Parameters, listaParametersProcedimiento);

            procedimientoAlmacenadoApi.DbCommand.Parameters.Clear();
            procedimientoAlmacenadoApi.DbCommand.CommandTimeout = 0;
            procedimientoAlmacenadoApi.DbCommand.CommandType = CommandType.StoredProcedure;
            procedimientoAlmacenadoApi.DbCommand.CommandText = nombreProcedimientoCompleto;
            procedimientoAlmacenadoApi.DbCommand.Parameters.AddRange(sqlParameters.ToArray());

            Result resultadoApi = new();

            using (var dr = await procedimientoAlmacenadoApi.DbCommand.ExecuteReaderAsync())
            {
                DatabaseMapping<T> mapeoBaseDatoApi = LecturaBaseData<T>(dr);
                resultadoApi = new Result()
                {
                    Data = mapeoBaseDatoApi.Records.ToList(),
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        ResponseMessage = ResponseMessageCorrecto(tipoEsquemaApi.ToResponseMessage(), resultadoApi.Data),
                        NotificationTypeId = NotificationType.Success,
                        DataCount = mapeoBaseDatoApi.TotalRecords
                    }
                };
            }

            return resultadoApi;
        }
        private static string ResponseMessageCorrecto(ResponseMessage tipoReplyToSolicitud, object? datosReplyToApi)
        {
            string mensajeReplyToCorrecto = tipoReplyToSolicitud.GetString();

            if (datosReplyToApi == null && tipoReplyToSolicitud == ResponseMessage.RetrievedSuccessfully)
            {
                mensajeReplyToCorrecto = "No se encontraron registros.";
            }
            else
            {
                if (datosReplyToApi is System.Collections.ICollection collection)
                {
                    if (collection.Count == 0 && tipoReplyToSolicitud == ResponseMessage.RetrievedSuccessfully)
                    {
                        mensajeReplyToCorrecto = "No se encontraron registros.";
                    }
                }
            }

            return mensajeReplyToCorrecto;
        }
        private static object ValidarParameterParaBd(object valor)
        {
            if (valor == null || valor.ToString() == string.Empty || valor.ToString() == "-1")
            {
                return DBNull.Value;
            }
            else
            {
                object? valor_temp = Convert.ChangeType(valor, valor.GetType());

                if (valor_temp == null)
                {
                    return DBNull.Value;
                }
                else
                {
                    if (valor.GetType().Name == "String")
                    {
                        string valor_temp_trim = (string)valor_temp;
                        valor_temp = valor_temp_trim.Trim();
                    }

                    return valor_temp;
                }

            }
        }
        private static DatabaseMapping<T> LecturaBaseData<T>(DbDataReader dbDataReader)
        {
            DatabaseMapping<T> mapeoBaseData = new();
            var listaDatos = new List<T>();
            Type typeParameterType = typeof(T);

            switch (typeParameterType.FullName)
            {
                case "System.Int32":
                case "System.String":
                case "System.Decimal":
                case "System.Single":
                case "System.DateTime":
                case "System.Boolean":
                    int contadorRecords = 0;
                    while (dbDataReader.Read())
                    {
                        T Entity = (T)dbDataReader[0];
                        listaDatos.Add(Entity);
                        contadorRecords++;
                    }
                    mapeoBaseData.Records = listaDatos;
                    mapeoBaseData.TotalRecords = contadorRecords;
                    break;
                default:
                    while (dbDataReader.Read())
                    {
                        var entidad = (T)Activator.CreateInstance(typeof(T));

                        for (int i = 0; i < dbDataReader.FieldCount; i++)
                        {
                            string rowName = dbDataReader.GetName(i);
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                // 1). If entity reference, bypass it.
                                if (prop.PropertyType.Namespace == typeof(T).Namespace)
                                {
                                    continue;
                                }
                                // 2). If collection, bypass it.
                                if (prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                                {
                                    continue;
                                }
                                // 3). If property is NotMapped, bypass it.
                                if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                                {
                                    continue;
                                }

                                if (prop.Name == rowName)
                                {
                                    var dbValue = dbDataReader[prop.Name];
                                    if (dbValue is DBNull) continue;
                                    if (prop.PropertyType.IsConstructedGenericType &&
                                        prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    {
                                        var baseType = prop.PropertyType.GetGenericArguments()[0];
                                        var baseValue = Convert.ChangeType(dbValue, baseType);
                                        var value = Activator.CreateInstance(prop.PropertyType, baseValue);

                                        if (prop.Name == "TotalRecords")
                                        {
                                            mapeoBaseData.TotalRecords = int.Parse(value.ToString());
                                            value = null;
                                        }

                                        prop.SetValue(entidad, value);

                                        break;
                                    }
                                    else
                                    {
                                        var value = Convert.ChangeType(dbValue, prop.PropertyType);

                                        if (prop.Name == "TotalRecords")
                                        {
                                            mapeoBaseData.TotalRecords = int.Parse(value.ToString());
                                            value = null;
                                        }

                                        prop.SetValue(entidad, value);

                                        break;
                                    }
                                }
                            }
                        }

                        listaDatos.Add(entidad);
                    }

                    if (mapeoBaseData.TotalRecords == null)
                    {
                        mapeoBaseData.TotalRecords = listaDatos.Count;
                    }
                    mapeoBaseData.Records = listaDatos;
                    break;
            }

            return mapeoBaseData;
        }
        private List<string> ListaParameters(object? obj)
        {
            List<string> vs = new();

            foreach (var propiedad in obj.GetType().GetProperties())
            {
                var name = "@" + propiedad.Name;
                var type = obj.GetPropertyTypeName(propiedad.Name);

                switch (type)
                {
                    case "Int32":
                    case "Int64":
                    case "String":
                    case "Decimal":
                    case "Single":
                    case "DateTime":
                    case "Boolean":
                        vs.Add(name);
                        break;
                    default:
                        break;
                }
            }

            return vs;
        }
        private List<SqlParameter> ListaParameters(object? obj, List<string> parametros)
        {
            var resultadoSqlParameter = new List<SqlParameter>();

            foreach (var parametro in parametros)
            {
                SqlParameter param = new();

                param.ParameterName = parametro;
                if (obj.GetType().GetProperty(parametro[1..]) == null) //parametro.Substring(1, parametro.Length - 1)
                {
                    foreach (var propiedad in obj.GetType().GetProperties())
                    {
                        var clase = obj.GetType().GetProperty(propiedad.Name).GetValue(obj, null);
                        if (clase != null && clase.GetType().GetProperty(parametro[1..]) != null)
                        {
                            param.Value = ValidarParameterParaBd(clase.GetType().GetProperty(parametro[1..]).GetValue(clase, null));
                        }
                    }
                }
                else
                {
                    param.Value = ValidarParameterParaBd(obj.GetType().GetProperty(parametro[1..]).GetValue(obj, null));
                }

                resultadoSqlParameter.Add(param);
            }

            return resultadoSqlParameter;
        }
        private string ParametersSqlConcatenados(List<SqlParameter> sqlParameters)
        {
            string cadenaValores = string.Empty;

            if (sqlParameters.Count > 0)
            {
                foreach (var parametro in sqlParameters)
                {
                    cadenaValores = cadenaValores + parametro.ParameterName + " = " + (parametro.Value == null ? "" : parametro.Value.ToString()) + ", ";
                }

                cadenaValores = cadenaValores[0..^2]; //cadenaValores.Substring(0, cadenaValores.Length - 2);
            }

            return cadenaValores;
        }

        public DbCommand Connection<D>()
        {
            DbContext dbContext = (DbContext)Activator.CreateInstance(typeof(D));
            //Validar
            if (dbContext == null)
            {
                throw _iExceptionFactory.Warning("La Conexión no fue enviada en Infraestructura.");
            }
            DbCommand dbCommand = dbContext.Database.GetDbConnection().CreateCommand();
            var conn = new DbConnectionHandler(dbCommand);
            return conn.Open();
        }

        public async Task<Result> Create<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand)
        {
            return await StoreProcedure<T>(SchemaType.Create, new ()
            {
                ProcedureName = storeProcedureName,
                Parameters = Parameters,
                DbCommand = dbCommand
            });
        }

        public async Task<Result> Read<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand)
        {
            return await StoreProcedure<T>(SchemaType.Read, new()
            {
                ProcedureName = storeProcedureName,
                Parameters = Parameters,
                DbCommand = dbCommand
            });
        }

        public async Task<Result> Update<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand)
        {
            return await StoreProcedure<T>(SchemaType.Update, new()
            {
                ProcedureName = storeProcedureName,
                Parameters = Parameters,
                DbCommand = dbCommand
            });
        }

        public async Task<Result> Delete<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand)
        {
            return await StoreProcedure<T>(SchemaType.Delete, new()
            {
                ProcedureName = storeProcedureName,
                Parameters = Parameters,
                DbCommand = dbCommand
            });
        }

        public async Task<Result> Activate<T>(string storeProcedureName, Parameter Parameters, DbCommand dbCommand)
        {
            return await StoreProcedure<T>(SchemaType.Activate, new()
            {
                ProcedureName = storeProcedureName,
                Parameters = Parameters,
                DbCommand = dbCommand
            });
        }
    }
}

