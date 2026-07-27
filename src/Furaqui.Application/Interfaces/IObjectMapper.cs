using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furaqui.Application.Interfaces
{
    public interface IObjectMapper
    {
        Target MapperSingle<Source, Target>(Source source);
        List<Target> MapperList<Source, Target>(List<Source> source);
    }
}
