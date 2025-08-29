using System;
using System.Collections.Generic;
using SpecialEducationPlanning
.Business.Model;

namespace SpecialEducationPlanning
.Api.Model.UserInfoModel
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfoModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AiepId"></param>
        /// <param name="ReleaseNotes"></param>
        /// <param name="User"></param>
        /// <param name="Roles"></param>
        /// <param name="Permissions"></param>
        /// /// <param name="Url"></param>
        public UserInfoModel(
            string AiepCode,
            int AiepId,
            int? currentAiepId,
            string currentAiepCode,
            UserModel user,
            IEnumerable<string> roles,
            IEnumerable<string> permissions,
            int? showReleaseInfoId,
            string url,
            string backendVersion,
            bool proToolEnabled)
        {
            this.AiepCode = AiepCode;
            this.AiepId = AiepId;
            this.CurrentAiepId = currentAiepId;
            this.CurrentAiepCode = currentAiepCode;
            this.User = user;
            this.Roles = roles;
            this.Permissions = permissions;
            this.ShowReleaseInfoId = showReleaseInfoId;
            this.releaseNotesURL = url;
            this.BackendVersion = backendVersion;
            this.ProToolEnabled = proToolEnabled;
        }

        /// <summary>
        /// Gets the current format date.
        /// </summary>
        [ObsoleteAttribute("Convert to i18n")]//TODO convert to i18n
        public string FormatDate { get; set; } = "dd/MM/yyyy";

        /// <summary>
        /// 
        /// </summary>
        public int AiepId { get; set; }

        public string AiepCode { get; set; }



        public int? CurrentAiepId { get; set; }

        public string CurrentAiepCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? ShowReleaseInfoId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserModel User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Roles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string releaseNotesURL { get; set; }

        public string BackendVersion { get; set; }

        public bool ProToolEnabled { get; set; }
    }
}

