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
            if (context.Error != null)
            {
                string errorMessage = context.Response.ErrorDescription ?? context.Error;
                if (!string.IsNullOrEmpty(context.Response.ErrorUri))
                {
                    errorMessage += $" - {context.Response.ErrorUri}";
                }
                throw _exceptionFactory.Error(errorMessage, StatusCodes.Status401Unauthorized);
            }
            else
            {
                return default;
            }
        }
    }
}

