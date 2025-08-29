using Hangfire.Common;
using Hangfire.States;
using Hangfire;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{
    public class NullBackgroundJobClient : IBackgroundJobClient
    {
        public string Create(Job job, IState state) => null;

        public bool ChangeState(string jobId, IState state, string expectedState) => false;
    }
}
