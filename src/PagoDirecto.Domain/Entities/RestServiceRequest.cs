using System.Collections.Generic;
using System.Net.Http;

namespace PagoDirecto.Domain.Entities;

public class RestServiceRequest
{
    public string UrlAddress { get; set; } = string.Empty;
    public RestBody? Body { get; set; }
    public HttpMethod RestMethod { get; set; } = HttpMethod.Get;
    public Dictionary<string, object>? QueryParams { get; set; }
    public Dictionary<string, object>? Headers { get; set; }
    public RestAuthentication? Authentication { get; set; }
    public bool IsResult { get; set; }
}

