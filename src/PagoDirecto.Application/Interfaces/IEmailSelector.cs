using PagoDirecto.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces
{
    public interface IEmailSelector
    {
        Task<Result> SendEmailAsync(Email correoApi, PagoDirecto.Domain.Enums.EmailHostType hostType, System.Threading.CancellationToken cancellationToken = default);
    }
}

