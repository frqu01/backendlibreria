using PagoDirecto.Application.Extensions;
using PagoDirecto.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PagoDirecto.Infrastructure.Repositories
{
    internal class ObjectMapperRepository : IObjectMapper
    {
        private readonly ILoggerFactory _iLoggerFactory;
        private static readonly ConcurrentDictionary<(Type, Type), IMapper> _mapperCache = new();

        public ObjectMapperRepository(ILoggerFactory iLoggerFactory)
        {
            _iLoggerFactory = iLoggerFactory;
        }

        private IMapper GetMapper<Source, Target>()
        {
            var key = (typeof(Source), typeof(Target));
            return _mapperCache.GetOrAdd(key, _ =>
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Target>(), _iLoggerFactory);
                return new Mapper(config);
            });
        }

        public List<Target> MapperList<Source, Target>(List<Source> source)
        {
            return GetMapper<Source, Target>().Map<List<Target>>(source);
        }

        public Target MapperSingle<Source, Target>(Source source)
        {
            return GetMapper<Source, Target>().Map<Target>(source);
        }
    }
}

