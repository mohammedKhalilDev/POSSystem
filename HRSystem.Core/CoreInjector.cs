using Microsoft.Extensions.DependencyInjection;
using System;

namespace HRSystem.Core
{
    public static class CoreInjector
    {
        public static IServiceCollection InjectCore(this IServiceCollection services)
        {
            return services;
        }
    }
}
