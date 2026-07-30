using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class EmailSelectorRepository : IEmailSelector
    {
        private readonly ILogger<EmailSelectorRepository> _logger;
        private readonly IResponseFactory _responseFactory;

        public EmailSelectorRepository(ILogger<EmailSelectorRepository> logger, IResponseFactory responseFactory)
        {
            _logger = logger;
            _responseFactory = responseFactory;
        }

        public async Task<Result> SendEmailAsync(Email correoApi, EmailHostType hostType, CancellationToken cancellationToken = default)
        {
            if (correoApi == null)
                return _responseFactory.Warning("No se enviaron los datos del correo.");

            if (string.IsNullOrWhiteSpace(correoApi.Sender))
                return _responseFactory.Warning("No se envió el emisor.");

            if (correoApi.Recipients == null || !correoApi.Recipients.Any())
                return _responseFactory.Warning("No se envió el receptor.");

            if (string.IsNullOrWhiteSpace(correoApi.Body))
                return _responseFactory.Warning("No se envió el cuerpo.");

            try
            {
                using var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(correoApi.Sender));
                message.Subject = correoApi.Subject ?? string.Empty;

                foreach (var item in correoApi.Recipients.Where(r => !string.IsNullOrWhiteSpace(r)))
                {
                    message.To.Add(MailboxAddress.Parse(item));
                }

                if (correoApi.ReplyTo != null)
                {
                    foreach (var item in correoApi.ReplyTo.Where(r => !string.IsNullOrWhiteSpace(r)))
                        message.ReplyTo.Add(MailboxAddress.Parse(item));
                }

                if (correoApi.Cc != null)
                {
                    foreach (var item in correoApi.Cc.Where(c => !string.IsNullOrWhiteSpace(c)))
                        message.Cc.Add(MailboxAddress.Parse(item));
                }

                if (correoApi.Bcc != null)
                {
                    foreach (var item in correoApi.Bcc.Where(b => !string.IsNullOrWhiteSpace(b)))
                        message.Bcc.Add(MailboxAddress.Parse(item));
                }

                var builder = new BodyBuilder { HtmlBody = correoApi.Body };

                if (correoApi.Attachments != null)
                {
                    foreach (var item in correoApi.Attachments)
                    {
                        if (item.AttachmentStream != null)
                        {
                            item.AttachmentStream.Position = 0; // Prevenir errores si el stream ya fue leído
                            var contentType = ContentType.Parse(item.FileExtensionType.GetDescription());
                            builder.Attachments.Add(item.FileName, item.AttachmentStream, contentType);
                        }
                    }
                }

                message.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(hostType.GetHost(), hostType.GetPort(), MailKit.Security.SecureSocketOptions.Auto, cancellationToken);
                
                if (!string.IsNullOrWhiteSpace(correoApi.Password))
                {
                    await smtp.AuthenticateAsync(correoApi.Sender, correoApi.Password, cancellationToken);
                }

                await smtp.SendAsync(message, cancellationToken);
                await smtp.DisconnectAsync(true, cancellationToken);

                return new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = true,
                        NotificationType = NotificationType.Success,
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
                        NotificationType = NotificationType.Error,
                        ResponseMessage = "Ocurrió un error al enviar el correo.",
                        ResponseMessageDetail = ex.Message
                    }
                };
            }
        }


    }
}

