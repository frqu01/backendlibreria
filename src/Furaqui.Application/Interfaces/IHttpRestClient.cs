using Furaqui.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces
{
    /// <summary>
    /// Servicio para el consumo de APIs REST de forma asíncrona.
    /// </summary>
    public interface IHttpRestClient
    {
        /// <summary>
        /// Consume un servicio REST asíncronamente.
        /// </summary>
        /// <param name="solicitudServicioApi">Parámetros de la solicitud HTTP.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Result de la ejecución envuelto en <see cref="Result"/>.</returns>
        Task<Result> SendAsync(RestServiceRequest solicitudServicioApi, CancellationToken cancellationToken = default);
    }
}
