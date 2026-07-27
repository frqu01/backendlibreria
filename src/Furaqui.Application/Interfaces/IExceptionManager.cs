using Furaqui.Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace Furaqui.Application.Interfaces
{
    public interface IExceptionManager
    {
        Task HandlerExceptionServer(StatusCodeContext context);
        Task HandlerExceptionApplication(HttpContext context);
        void ExceptionSaveRecord(ChangeTracker changeTracker);
    }
}
