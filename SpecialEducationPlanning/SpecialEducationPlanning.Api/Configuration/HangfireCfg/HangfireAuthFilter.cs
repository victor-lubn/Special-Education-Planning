using Hangfire.Dashboard;

namespace SpecialEducationPlanning
.Api.Configuration.HangfireCfg
{

    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {

        #region Methods IDashboardAuthorizationFilter

        public bool Authorize(DashboardContext context)
        {
            return true;
        }

        #endregion

    }

}