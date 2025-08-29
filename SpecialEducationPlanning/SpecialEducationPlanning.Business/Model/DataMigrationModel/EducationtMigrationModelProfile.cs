using AutoMapper;
using System;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class AiepMigrationModelProfile : BaseProfile<AiepMigrationModel, Aiep, int>
    {

        protected override IMappingExpression<Aiep, AiepMigrationModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(m => m.TdpId, o => o.MapFrom(e => e.Id));
        }

        protected override IMappingExpression<AiepMigrationModel, Aiep> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(b => b.Id, opt => opt.Ignore())
                .ForMember(e => e.AiepCode, o => o.MapFrom(s => s.AiepNmbr.LimitTo(20)))
                .ForMember(e => e.Name, o => o.MapFrom(s => s.AiepName.LimitTo(100)))
                .ForMember(e => e.Area, o => o.Ignore()) // Mapped from the Aiep Migration Service since in TDP this is an EF relationship
                .ForMember(e => e.Manager, o => o.Ignore()) // Mapped from the Aiep Migration Service since in TDP this is an EF relationship
                .ForMember(e => e.Email, o => o.MapFrom(s => s.EmailAddress.LimitTo(100)))
                .ForMember(e => e.Address1, o => o.MapFrom(s => s.Address1.LimitTo(100)))
                .ForMember(e => e.Address2, o => o.MapFrom(s => s.Address2.LimitTo(100)))
                .ForMember(e => e.Address3, o => o.MapFrom(s => s.Address3.LimitTo(100)))
                .ForMember(e => e.Address4, o => o.MapFrom(s => s.Address4.LimitTo(100)))
                .ForMember(e => e.Address5, o => o.MapFrom(s => s.Address5.LimitTo(100)))
                .ForMember(e => e.Postcode, o => o.MapFrom(s => s.PostCode.LimitTo(20)))
                .ForMember(e => e.PhoneNumber, o => o.MapFrom(s => s.Telephone.LimitTo(20)))
                ;
        }
    }

    public static class MappingExtensions
    {
        public static string LimitTo(this string value, int length)
        {
            // return value?.Length <= length ? value : value.Substring(0, length) ?? null;

            if (value.IsNullOrEmpty())
            {
                return null;
            }
            else if (value.Length <= length)
            {
                return value;
            }
            else
            {
                return value.Substring(0, length);
            }
        }
    }
}

