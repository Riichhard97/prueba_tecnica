using System;
using System.Reflection;
using AutoMapper;
using Nexu.Shared.Model;

namespace Nexu.Shared.Infrastructure
{
    public static class AutoMapperConfig
    {
        public static void Configure(IMapperConfigurationExpression cfg, params Assembly[] assemblies)
        {
            if (cfg is null)
            {
                throw new ArgumentNullException(nameof(cfg));
            }

            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (assemblies.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(assemblies));
            }

            cfg.AddMaps(assemblies);
            cfg.ForAllMaps(AutoIgnorePropertiesInternal);
        }

        private static void AutoIgnorePropertiesInternal(TypeMap map, IMappingExpression expression)
        {
            if (typeof(IHaveDateCreated).IsAssignableFrom(map.DestinationType))
            {
                expression.ForMember(nameof(IHaveDateCreated.DateCreated), e => e.Ignore());
            }

            if (typeof(IHaveDateUpdated).IsAssignableFrom(map.DestinationType))
            {
                expression.ForMember(nameof(IHaveDateUpdated.DateUpdated), e => e.Ignore());
            }
        }
    }
}
