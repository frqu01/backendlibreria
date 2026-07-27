using Furaqui.Application.Extensions;
using Furaqui.Domain.Entities;
using Furaqui.Domain.Enums;
using Furaqui.Application.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Infrastructure.Repositories
{
    internal class EmailSelectorRepository : IEmailSelector
    {
        public Result Gmail(Email correoApi)
        {
            Result resultadoApi;

            if (correoApi == null)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió los datos del correo."
                    }
                };

                return resultadoApi;
            };

            if (correoApi.Sender == string.Empty)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió el emisor."
                    }
                };

                return resultadoApi;
            }

            if (correoApi.Recipients == null || correoApi.Recipients.Count < 1)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió el receptor."
                    }
                };

                return resultadoApi;
            }

            if (correoApi.Body == string.Empty)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió el cuerpo."
                    }
                };

                return resultadoApi;
            }

            var message = new MailMessage();

            SmtpClient smtp = new SmtpClient()
            {
                Host = EmailHostType.Outlook.GetHost(),
                Port = EmailHostType.Outlook.GetPort(),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(correoApi.Sender, correoApi.Password)
            };

            foreach (var item in correoApi.Recipients)
            {
                message = new MailMessage(correoApi.Sender, item);
            }

            foreach (var item in correoApi.ReplyTo)
            {
                message.ReplyToList.Add(item);
            }

            foreach (var item in correoApi.Cc)
            {
                message.CC.Add(item);
            }

            foreach (var item in correoApi.Cc)
            {
                message.CC.Add(item);
            }

            foreach (var item in correoApi.Bcc)
            {
                message.CC.Add(item);
            }

            foreach (var item in correoApi.Attachments)
            {
                //var filePath = "D:\\Documentos\\Frank\\Trabajos\\MDP\\Clonfluence - PAGDES-200522-2131.pdf";
                //var provider = new FileExtensionContentTypeProvider();

                //if (!provider.TryGetContentType(filePath, out var contentType))
                //{
                //    contentType = "application/octet-stream";
                //}

                message.Attachments.Add(new Attachment(item.AttachmentStream, item.FileName, item.FileExtensionType.GetDescription()));
            }

            message.IsBodyHtml = true;
            message.Body = correoApi.Body;
            smtp.Send(message);

            resultadoApi = new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = true,
                    NotificationTypeId = NotificationType.Success,
                    ResponseMessage = "Email enviado correctamente."
                }
            };

            return resultadoApi;
        }

        public Result Outlook(Email correoApi)
        {
            Result resultadoApi;

            if (correoApi == null)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió los datos del correo."
                    }
                };

                return resultadoApi;
            };

            if (correoApi.Sender == string.Empty)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió el emisor."
                    }
                };

                return resultadoApi;
            }

            if (correoApi.Recipients == null || correoApi.Recipients.Count < 1)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió el receptor."
                    }
                };

                return resultadoApi;
            }

            if (correoApi.Body == string.Empty)
            {
                resultadoApi = new Result()
                {
                    RequestStatus = new RequestStatus()
                    {
                        IsSuccess = false,
                        NotificationTypeId = NotificationType.Warning,
                        ResponseMessage = "No se envió el cuerpo."
                    }
                };

                return resultadoApi;
            }

            var message = new MailMessage();

            SmtpClient smtp = new SmtpClient()
            {
                Host = EmailHostType.Gmail.GetHost(),
                Port = EmailHostType.Gmail.GetPort(),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(correoApi.Sender, correoApi.Password)
            };

            foreach (var item in correoApi.Recipients)
            {
                message = new MailMessage(correoApi.Sender, item);
            }

            foreach (var item in correoApi.ReplyTo)
            {
                message.ReplyToList.Add(item);
            }

            //foreach (var item in correoApi.Cc)
            //{
            //    message.CC.Add(item);
            //}

            foreach (var item in correoApi.Cc)
            {
                message.CC.Add(item);
            }

            foreach (var item in correoApi.Bcc)
            {
                message.CC.Add(item);
            }

            foreach (var item in correoApi.Attachments)
            {
                message.Attachments.Add(new Attachment(item.AttachmentStream, item.FileName, item.FileExtensionType.GetDescription()));
            }

            message.IsBodyHtml = true;
            message.Body = correoApi.Body;
            smtp.Send(message);

            resultadoApi = new Result()
            {
                RequestStatus = new RequestStatus()
                {
                    IsSuccess = true,
                    NotificationTypeId = NotificationType.Success,
                    ResponseMessage = "Email enviado correctamente."
                }
            };

            return resultadoApi;
        }
    }
}
