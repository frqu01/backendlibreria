using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using OpenIddict.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class OpenIddictServerHandlerRepository : IOpenIddictServerHandler<ApplyTokenResponseContext>
    {
        private readonly IExceptionFactory _exceptionFactory;
        public OpenIddictServerHandlerRepository(IExceptionFactory exceptionFactory)
        {
            _exceptionFactory = exceptionFactory;
        }
        public ValueTask HandleAsync(ApplyTokenResponseContext context)
        {
            //Se envía a la librería que capta los errores
            if (context.Error != null)
            {
                throw _exceptionFactory.Error(context.Response.ErrorDescription + " - " + context.Response.ErrorUri, StatusCodes.Status401Unauthorized);
            }
            else
            {
                return default;
            }
        }
    }
}

