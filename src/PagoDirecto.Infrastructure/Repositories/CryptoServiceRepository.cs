using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using PagoDirecto.Application.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class CryptoServiceRepository : ICryptoService
    {
        protected readonly IDataProtectionProvider _iDataProtectionProvider;
        protected readonly CryptoOptions _dataProtectionOptions;
        private readonly ILogger<CryptoServiceRepository> _logger;
        
        public CryptoServiceRepository(
            IDataProtectionProvider iDataProtectionProvider, 
            IOptions<CryptoOptions> dataProtectionOptions,
            ILogger<CryptoServiceRepository> logger)
        {
            _iDataProtectionProvider = iDataProtectionProvider;
            _dataProtectionOptions = dataProtectionOptions.Value;
            _logger = logger;
        }

        public Task<Result> DecryptAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Task.FromResult(ErrorResult("El texto a descifrar no puede estar vacío."));

            try
            {
                string purpose = string.IsNullOrWhiteSpace(_dataProtectionOptions.EncryptKey) 
                    ? "PagoDirecto.DefaultProtection" : _dataProtectionOptions.EncryptKey;
                var dataProtectionProvider = _iDataProtectionProvider.CreateProtector(purpose);

                var resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationTypeId = NotificationType.Success,
                        ResponseMessage = "Se desencriptó correctamente."
                    },
                    Data = dataProtectionProvider.Unprotect(texto)
                };

                return Task.FromResult(resultadoApi);
            }
            catch (CryptographicException ex)
            {
                _logger.LogWarning(ex, "Fallo al descifrar el texto. Posible alteración, corrupción de datos o llave revocada.");
                return Task.FromResult(ErrorResult("El texto proporcionado no es válido o está corrupto."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo inesperado al intentar descifrar el texto.");
                return Task.FromResult(ErrorResult("Ocurrió un error inesperado durante el descifrado."));
            }
        }

        public Task<Result> EncryptAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return Task.FromResult(ErrorResult("El texto a encriptar no puede estar vacío."));

            try
            {
                string purpose = string.IsNullOrWhiteSpace(_dataProtectionOptions.EncryptKey) 
                    ? "PagoDirecto.DefaultProtection" : _dataProtectionOptions.EncryptKey;
                var dataProtectionProvider = _iDataProtectionProvider.CreateProtector(purpose);

                var resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationTypeId = NotificationType.Success,
                        ResponseMessage = "Se encriptó correctamente."
                    },
                    Data = dataProtectionProvider.Protect(texto)
                };

                return Task.FromResult(resultadoApi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo inesperado al intentar encriptar el texto.");
                return Task.FromResult(ErrorResult("Ocurrió un error inesperado durante la encriptación."));
            }
        }

        private static Result ErrorResult(string message)
        {
            return new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    NotificationTypeId = NotificationType.Error,
                    ResponseMessage = message
                }
            };
        }
    }
}

