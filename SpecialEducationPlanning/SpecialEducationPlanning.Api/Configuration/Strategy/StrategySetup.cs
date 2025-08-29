using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialEducationPlanning
.Api.Configuration.Strategy
{
    public static class StrategySetup
    {
        /// <summary>
        /// Selects a concrete implementation for the passed Type parameter based on the beginning of the concrete implementation's name
        /// </summary>
        /// <typeparam name="TStrategy"></typeparam>
        /// <param name="services"></param>
        /// <param name="strategyIdentifier"></param>
        public static void AddStrategy<TStrategy>(this IServiceCollection services, string strategyIdentifier)
        {
            var strategy = AppDomain.CurrentDomain.GetAssemblies() // From the assembly
            .SelectMany(a => a.GetTypes() // Get the classes that implement the passed type (TStrategy)
            .Where(t => typeof(TStrategy).IsAssignableFrom(t) && t.Name.StartsWith(strategyIdentifier))) //And select the one that begins with the passed strategyIdentifier (e.g. "Uk")
            .FirstOrDefault();
            // Register the concrete implementation to the passed Type parameter
            services.AddScoped(typeof(TStrategy), strategy);
        }
    }
}
