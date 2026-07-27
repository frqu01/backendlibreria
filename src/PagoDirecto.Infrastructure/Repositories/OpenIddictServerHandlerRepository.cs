using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OpenIddictServerHandlerRepository> _logger;

        public OpenIddictServerHandlerRepository(IExceptionFactory exceptionFactory, ILogger<OpenIddictServerHandlerRepository> logger)
        {
            _exceptionFactory = exceptionFactory;
            _logger = logger;
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

                _logger.LogWarning("Fallo de autenticación OpenIddict: {Error} - {Description}", context.Error, errorMessage);
                throw _exceptionFactory.Error(errorMessage, StatusCodes.Status401Unauthorized);
            }
            else
            {
                return default;
            }
        }
    }
}

