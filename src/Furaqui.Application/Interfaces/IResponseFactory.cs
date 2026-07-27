using Furaqui.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces
{
    public interface IResponseFactory
    {
        public Result Success();
        public Result Success(string mensaje, object? dato);
        public Result Success(object? dato);
        public Result Information();
        public Result Information(object? dato);
        public Result Information(string mensaje, object? dato);
        public Result Warning();
        public Result Warning(object? dato);
        public Result Warning(string mensaje, object? dato);
        public Result Error();
        public Result Error(object? dato);
        public Result Error(string mensaje, object? dato);
        public Result AlreadyExist(string campo);
        public Result AlreadyExist();
        public Result AlreadyExist(object? dato);
        public Result NotExist(string campo);
        public Result NotExist();
        public Result CreateOk();
        public Result CreateOk(int id);
        public Result CreateOk(long id);
        public Result CreateOk(string id);
        public Result CreateOk(List<int> ids);
        public Result CreateOk(List<long> ids);
        public Result CreateOk(List<string> ids);
        public Result ReadOk();
        public Result ReadOk(object? dato);
        public Result ReadOk(object? dato, int cantidadDatos);
        public Result ReadByIdOk(object? dato);
        public Result UpdateOk();
        public Result DeleteOk();
        public Result ActivateOk();
        public Result New(Result resultadoApi);
        public Result ErrorValidate(string campo, string mensaje);
    }
}
