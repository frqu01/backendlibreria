using Furaqui.Application.Extensions;
using Furaqui.Domain.Entities;
using Furaqui.Domain.Enums;
using Furaqui.Application.Interfaces;
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

namespace Furaqui.Infrastructure.Repositories
{
    internal class HttpRestClientRepository : IHttpRestClient
    {
        protected readonly IResponseFactory _iResponseApi;
        protected readonly IHttpClientFactory _httpClientFactory;

        public HttpRestClientRepository(
            IResponseFactory iResponseApi, 
            IHttpClientFactory httpClientFactory)
        {
            _iResponseApi = iResponseApi;
            _httpClientFactory = httpClientFactory;
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

            HttpClient client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            if (solicitudServicioApi.Headers != null)
            {
                foreach (KeyValuePair<string, object> entry in solicitudServicioApi.Headers)
                {
                    client.DefaultRequestHeaders.Add(entry.Key, entry.Value?.ToString());
                }
            }

            if (solicitudServicioApi.Authentication != null)
            {
                autenticationEncrypt = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", solicitudServicioApi.Authentication.Username, solicitudServicioApi.Authentication.Password)));

                if (solicitudServicioApi.Authentication.AuthorizationType == RestAuthorizationType.BasicAuth)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", autenticationEncrypt);
                }
            }

            HttpContent httpContent = null;

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
                    httpContent = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)solicitudServicioApi.Body.Payload);
                }
            }

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(solicitudServicioApi.RestMethod, url)
            {
                Content = httpContent
            };

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = await client.SendAsync(httpRequestMessage, cancellationToken);
            }
            catch (Exception ex)
            {
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
