using PagoDirecto.Application.Extensions;
using PagoDirecto.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class ObjectMapperRepository : IObjectMapper
    {
        protected readonly ILoggerFactory _iLoggerFactory;
        public ObjectMapperRepository(ILoggerFactory iLoggerFactory)
        {
            _iLoggerFactory = iLoggerFactory;
        }
        public List<Target> MapperList<Source, Target>(List<Source> source)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Target>(), _iLoggerFactory);
            return new Mapper(config).Map<List<Target>>(source);
        }

        public Target MapperSingle<Source, Target>(Source source)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Target>(), _iLoggerFactory);
            return new Mapper(config).Map<Target>(source);
        }
    }
}

