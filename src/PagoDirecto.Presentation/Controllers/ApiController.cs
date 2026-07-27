using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PagoDirecto.Domain.Entities;
using PagoDirecto.Domain.Enums;

namespace PagoDirecto.Presentation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private ISender? _iMediator;
        protected ISender _mediator => _iMediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected IActionResult CustomResponse(Result result)
        {
            if (result.IsSuccessful())
            {
                return Ok(result);
            }

            if (result.RequestStatus?.NotificationType == NotificationType.Warning)
            {
                return BadRequest(result);
            }

            if (result.RequestStatus?.NotificationType == NotificationType.Error)
            {
                return StatusCode(500, result);
            }

            return BadRequest(result);
        }
    }
}
