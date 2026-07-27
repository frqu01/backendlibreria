using PagoDirecto.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Interfaces
{
    /// <summary>
    /// Servicio para el consumo de APIs REST de forma asÃ­ncrona.
    /// </summary>
    public interface IHttpRestClient
    {
        /// <summary>
        /// Consume un servicio REST asÃ­ncronamente.
        /// </summary>
        /// <param name="solicitudServicioApi">ParÃ¡metros de la solicitud HTTP.</param>
        /// <param name="cancellationToken">Token de cancelaciÃ³n opcional.</param>
        /// <returns>Result de la ejecuciÃ³n envuelto en <see cref="Result"/>.</returns>
        Task<Result> SendAsync(RestServiceRequest solicitudServicioApi, CancellationToken cancellationToken = default);
    }
}

