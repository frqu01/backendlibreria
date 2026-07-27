using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Furaqui.Presentation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private ISender _iMediator;
        protected ISender _mediator => _iMediator ??= HttpContext.RequestServices.GetService<ISender>();
    }
}
