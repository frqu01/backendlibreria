using Furaqui.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces
{
    public interface IEmailSelector
    {
        Result Outlook(Email correoApi);
        Result Gmail(Email correoApi);
    }
}
