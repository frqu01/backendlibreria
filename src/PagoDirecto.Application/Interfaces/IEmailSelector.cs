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
        Task<Result> Outlook(Email correoApi, System.Threading.CancellationToken cancellationToken = default);
        Task<Result> Gmail(Email correoApi, System.Threading.CancellationToken cancellationToken = default);
    }
}

