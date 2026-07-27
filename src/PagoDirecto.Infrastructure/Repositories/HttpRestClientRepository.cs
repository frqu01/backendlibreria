using PagoDirecto.Application.Extensions;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;
using PagoDirecto.Application.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class HttpRestClientRepository : IHttpRestClient
    {
        protected readonly IResponseFactory _iResponseApi;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly Microsoft.Extensions.Logging.ILogger<HttpRestClientRepository> _logger;

        public HttpRestClientRepository(
            IResponseFactory iResponseApi, 
            IHttpClientFactory httpClientFactory,
            Microsoft.Extensions.Logging.ILogger<HttpRestClientRepository> logger)
        {
            _iResponseApi = iResponseApi;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Result> SendAsync(RestServiceRequest solicitudServicioApi, CancellationToken cancellationToken = default)
        {
            var resultadoApi = _iResponseApi.Success("Ejecutado correctamente.");
            string url = solicitudServicioApi.UrlAddress;
            string response = string.Empty;
            string paramsQuery = string.Empty;
            string autenticationEncrypt = string.Empty;

            if (solicitudServicioApi.QueryParams != null && solicitudServicioApi.QueryParams.Any())
            {
                paramsQuery = string.Join("&",
                solicitudServicioApi.QueryParams.Select(kvp =>
                    string.Format(
                        "{0}={1}",
                        kvp.Key,
                        HttpUtility.UrlEncode(kvp.Value?.ToString() ?? string.Empty))));

                url = url + "?" + paramsQuery;
            }

            HttpContent? httpContent = null;

            if (solicitudServicioApi.Body != null)
            {
                if (solicitudServicioApi.Body.BodyType == RestBodyType.RawJson)
                {
                    httpContent = new StringContent(JsonConvert.SerializeObject(solicitudServicioApi.Body.Payload, Newtonsoft.Json.Formatting.None,
                                        new JsonSerializerSettings
                                        {
                                            NullValueHandling = NullValueHandling.Ignore
                                        }), Encoding.UTF8, RestBodyType.RawJson.GetString());
                }

                if (solicitudServicioApi.Body.BodyType == RestBodyType.FormData)
                {
                    IEnumerable<KeyValuePair<string, string>>? nameValueCollection = null;
                    if (solicitudServicioApi.Body.Payload is IEnumerable<KeyValuePair<string, string>> kvpList)
                    {
                        nameValueCollection = kvpList;
                    }
                    else if (solicitudServicioApi.Body.Payload != null)
                    {
                        // Fallback seguro usando serialización a diccionario
                        var json = JsonConvert.SerializeObject(solicitudServicioApi.Body.Payload);
                        nameValueCollection = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    }
                    
                    if (nameValueCollection != null)
                    {
                        httpContent = new FormUrlEncodedContent(nameValueCollection);
                    }
                }
            }

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(solicitudServicioApi.RestMethod, url)
            {
                Content = httpContent
            };

            httpRequestMessage.Headers.Accept.Clear();

            if (solicitudServicioApi.Headers != null)
            {
                foreach (KeyValuePair<string, object> entry in solicitudServicioApi.Headers)
                {
                    httpRequestMessage.Headers.Add(entry.Key, entry.Value?.ToString());
                }
            }

            if (solicitudServicioApi.Authentication != null)
            {
                if (solicitudServicioApi.Authentication.AuthorizationType == RestAuthorizationType.BasicAuth)
                {
                    autenticationEncrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", solicitudServicioApi.Authentication.Username, solicitudServicioApi.Authentication.Password)));
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", autenticationEncrypt);
                }
                else if (solicitudServicioApi.Authentication.AuthorizationType == RestAuthorizationType.BearerToken)
                {
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", solicitudServicioApi.Authentication.Token);
                }
            }

            HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = await client.SendAsync(httpRequestMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico de red al consumir el servicio REST en la URL: {Url}", url);

                resultadoApi.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = "Error al consumir servicio",
                    NotificationTypeId = NotificationType.Error,
                    ResponseMessageDetail = ex.Message
                };

                resultadoApi.Data = null;

                return resultadoApi;
            }

            response = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogWarning("La API externa en {Url} devolvió código HTTP {StatusCode}. Detalle: {Response}", url, (int)httpResponseMessage.StatusCode, response);

                resultadoApi.RequestStatus = new RequestStatus()
                {
                    IsSuccess = false,
                    ResponseMessage = $"Error de la API externa (HTTP {(int)httpResponseMessage.StatusCode})",
                    NotificationTypeId = NotificationType.Error,
                    ResponseMessageDetail = response 
                };
                resultadoApi.Data = null;
                return resultadoApi;
            }

            resultadoApi.Data = response;

            if (solicitudServicioApi.IsResult && !string.IsNullOrEmpty(response))
            {
                resultadoApi = JsonConvert.DeserializeObject<Result>(response) ?? resultadoApi;
                resultadoApi.Data = resultadoApi.Data?.ToString();
            }

            return resultadoApi;
        }
    }
}

