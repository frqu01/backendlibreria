using PagoDirecto.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces
{
    public interface ICryptoService
    {
        Task<Result> DecryptAsync(string texto);
        Task<Result> EncryptAsync(string texto);
    }
}

