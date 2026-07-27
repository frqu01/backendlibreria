using PagoDirecto.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces
{
    public interface ILogRecorder
    {
        Result Success(string contenido);
        Result Information(string contenido);
        Result Warning(string contenido);
        Result Error(string contenido);
    }
}

