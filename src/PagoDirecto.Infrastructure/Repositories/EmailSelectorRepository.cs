using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class EmailSelectorRepository : IEmailSelector
    {
        private readonly ILogger<EmailSelectorRepository> _logger;

        public EmailSelectorRepository(ILogger<EmailSelectorRepository> logger)
        {
            _logger = logger;
        }

        public Task<Result> Gmail(Email correoApi, CancellationToken cancellationToken = default)
        {
            return SendEmailAsync(correoApi, EmailHostType.Gmail, cancellationToken);
        }

        public Task<Result> Outlook(Email correoApi, CancellationToken cancellationToken = default)
        {
            return SendEmailAsync(correoApi, EmailHostType.Outlook, cancellationToken);
        }

        private async Task<Result> SendEmailAsync(Email correoApi, EmailHostType hostType, CancellationToken cancellationToken = default)
        {
            if (correoApi == null)
                return ErrorResult("No se enviaron los datos del correo.");

            if (string.IsNullOrWhiteSpace(correoApi.Sender))
                return ErrorResult("No se envió el emisor.");

            if (correoApi.Recipients == null || !correoApi.Recipients.Any())
                return ErrorResult("No se envió el receptor.");

            if (string.IsNullOrWhiteSpace(correoApi.Body))
                return ErrorResult("No se envió el cuerpo.");

            try
            {
                using var message = new MailMessage();
                message.From = new MailAddress(correoApi.Sender);
                message.Subject = correoApi.Subject ?? string.Empty;
                message.IsBodyHtml = true;
                message.Body = correoApi.Body;

                foreach (var item in correoApi.Recipients.Where(r => !string.IsNullOrWhiteSpace(r)))
                {
                    message.To.Add(item);
                }

                if (correoApi.ReplyTo != null)
                {
                    foreach (var item in correoApi.ReplyTo.Where(r => !string.IsNullOrWhiteSpace(r)))
                        message.ReplyToList.Add(item);
                }

                if (correoApi.Cc != null)
                {
                    foreach (var item in correoApi.Cc.Where(c => !string.IsNullOrWhiteSpace(c)))
                        message.CC.Add(item);
                }

                if (correoApi.Bcc != null)
                {
                    foreach (var item in correoApi.Bcc.Where(b => !string.IsNullOrWhiteSpace(b)))
                        message.Bcc.Add(item);
                }

                if (correoApi.Attachments != null)
                {
                    foreach (var item in correoApi.Attachments)
                    {
                        if (item.AttachmentStream != null)
                        {
                            item.AttachmentStream.Position = 0; // Prevenir errores si el stream ya fue leído
                            message.Attachments.Add(new Attachment(item.AttachmentStream, item.FileName, item.FileExtensionType.GetDescription()));
                        }
                    }
                }

                using var smtp = new SmtpClient()
                {
                    Host = hostType.GetHost(),
                    Port = hostType.GetPort(),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(correoApi.Sender, correoApi.Password)
                };

                await smtp.SendMailAsync(message, cancellationToken);

                return new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationTypeId = NotificationType.Success,
                        ResponseMessage = "Email enviado correctamente."
                    }
                };
            }
            catch (Exception ex)
            {
                string destination = correoApi?.Recipients?.FirstOrDefault() ?? "Desconocido";
                _logger.LogError(ex, "Error al intentar enviar correo vía SMTP a {Receptor}", destination);

                return new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Error,
                        ResponseMessage = "Ocurrió un error al enviar el correo.",
                        ResponseMessageDetail = ex.Message
                    }
                };
            }
        }

        private static Result ErrorResult(string message)
        {
            return new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    NotificationTypeId = NotificationType.Warning,
                    ResponseMessage = message
                }
            };
        }
    }
}

