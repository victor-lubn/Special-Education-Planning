using System.Reflection;
using Xunit.Sdk;

namespace SpecialEducationPlanning
.Api.WebTest
{
    public class ResetDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            CompositionRootFixture.ResetDatabase();
        }
    }
}