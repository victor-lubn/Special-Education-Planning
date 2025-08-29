using Koa.Domain.Search.Page;
using Koa.Domain.Search.Sort;
using Koa.Domain.Specification;
using Koa.Persistence.Abstractions.Query;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Business.Query
{
    public class AiepMaterializedAiepModelPagedValueQuery : MultipleMaterializedPagedValueQuery<Aiep, AiepModel>
    {
        public AiepMaterializedAiepModelPagedValueQuery(ISpecification<Aiep> specification, ISpecification<AiepModel> Aieps,
            ICollection<SortDescriptor> sortDescriptor, IPageSize pageSize) : base(specification, Aieps, sortDescriptor, pageSize)
        {
        }

        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }

        protected override IQueryable<AiepModel> Materialize(IQueryable<Aiep> queryable)
        {
            var materialize = queryable.Select(c => new AiepModel
            {
                Id = c.Id,
                AiepCode = c.AiepCode,
                Name = c.Name,
                Email = c.Email,
                Address1 = c.Address1,
                Address2 = c.Address2,
                Address3 = c.Address3,
                Address4 = c.Address4,
                Address5 = c.Address5,
                Address6 = c.Address6,
                Postcode = c.Postcode,
                PhoneNumber = c.PhoneNumber,
                FaxNumber = c.FaxNumber,
                IpAddress = c.IpAddress,
                MediaServer = c.MediaServer,
                HtmlEmail = c.HtmlEmail,
                DownloadSpeed = c.DownloadSpeed,
                DownloadLimit = c.DownloadLimit,
                AreaId = c.AreaId,
                Area = new AreaModel
                {
                    KeyName = c.Area.KeyName
                },
                Educationers = c.Educationers.Select(ur => new UserModel
                {
                    AiepId = ur.AiepId,

                }).ToList(),
                UpdatedDate = c.UpdatedDate
            });
            return materialize;
        }
    }
}

