using System;

namespace SpecialEducationPlanning
.Business.Test
{
    public class TestServiceProviderAccessor
    {
        public TestServiceProviderAccessor(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }
    }
}