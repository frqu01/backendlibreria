using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class CryptoServiceRepository : ICryptoService
    {
        protected readonly IDataProtectionProvider _iDataProtectionProvider;
        protected readonly IConfiguration _iConfiguration;
        public CryptoServiceRepository(IDataProtectionProvider iDataProtectionProvider, 
            IConfiguration iConfiguration)
        {
            _iDataProtectionProvider = iDataProtectionProvider;
            _iConfiguration = iConfiguration;
        }
        public Task<Result> DecryptAsync(string texto)
        {
            var dataProtectionProvider = _iDataProtectionProvider.CreateProtector(_iConfiguration.GetValue<string>("DataProtection:EncryptKey"));

            var resultadoApi = new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = true,
                    NotificationTypeId = NotificationType.Success,
                    ResponseMessage = "Se desencriptó correctamente."
                },
                Data = texto == null ? null : dataProtectionProvider.Unprotect(texto)
            };

            return Task.FromResult(resultadoApi);
        }

        public Task<Result> EncryptAsync(string texto)
        {
            var dataProtectionProvider = _iDataProtectionProvider.CreateProtector(_iConfiguration.GetValue<string>("DataProtection:EncryptKey"));

            var resultadoApi = new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = true,
                    NotificationTypeId = NotificationType.Success,
                    ResponseMessage = "Se encriptó correctamente."
                },
                Data = texto == null ? null : dataProtectionProvider.Protect(texto)
            };

            return Task.FromResult(resultadoApi);
        }
    }
}

