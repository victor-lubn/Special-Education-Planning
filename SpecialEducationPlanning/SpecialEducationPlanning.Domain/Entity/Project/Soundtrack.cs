using Koa.Domain;
using System;

namespace SpecialEducationPlanning
.Domain.Entity
{
    public class Soundtrack : BaseEntity<int>
    {
        public DateTime CreatedDate { get; set; }
        public string Display { get; set; }
        public string Code { get; set; }
    }
}