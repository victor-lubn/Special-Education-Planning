using Koa.Domain;
using System;
using System.Collections.Generic;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class ReleaseInfo : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Version { get; set; }
        public string FusionVersion { get; set; }
        public DateTime DateTime { get; set; }
        public string Document { get; set; }
        public string DocumentPath { get; set; }
        public virtual IEnumerable<Aiep> Aieps { get; set; }
        public virtual IEnumerable<User> Educationers { get; set; }
        public virtual IEnumerable<UserReleaseInfo> UserReleasesInfo { get; set; }
    }
}


