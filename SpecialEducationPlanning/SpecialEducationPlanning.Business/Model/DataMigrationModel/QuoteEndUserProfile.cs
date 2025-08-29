using AutoMapper;
using System;
using System.Collections.Generic;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    public class QuoteEndUserProfile : BaseProfile<QuoteMigrationModel, EndUser, int>
    {
        protected override IMappingExpression<QuoteMigrationModel, EndUser> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.Surname, opt => opt.MapFrom(m => m.ConsumerSurname))
                .ForMember(e => e.FirstName, opt => opt.MapFrom(m => m.ConsumerInitials))
                //.ForMember(e => e.Title, opt => opt.MapFrom(m => m.ConsumerTitleInitials))
                .ForMember(e => e.Address0, opt => opt.MapFrom(m => m.ConsumerHouseNo + m.ConsumerHouse))
                .ForMember(e => e.Address1, opt => opt.MapFrom(m => new List<string>() { m.ConsumerPlot, m.ConsumerHouse, m.ConsumerHouseNo, m.ConsumerAddress1 }.ComposeAddress()))
                .ForMember(e => e.Address2, opt => opt.MapFrom(m => m.ConsumerAddress2))
                .ForMember(e => e.Address3, opt => opt.MapFrom(m => m.ConsumerAddress3))
                .ForMember(e => e.Address4, opt => opt.MapFrom(m => m.ConsumerAddress4))
                .ForMember(e => e.Address5, opt => opt.MapFrom(m => m.ConsumerAddress5))
                .ForMember(e => e.ContactEmail, opt => opt.MapFrom(m => m.ConsumerEmail))
                .ForMember(e => e.MobileNumber, opt => opt.MapFrom(m => m.ConsumerTele1))
                .ForMember(e => e.CreatedDate, opt => opt.MapFrom(s => DateTime.MaxValue))
                .ForMember(e => e.UpdatedDate, opt => opt.MapFrom(s => DateTime.UtcNow))
                .ForMember(e => e.LandLineNumber, opt => opt.MapFrom(m => m.ConsumerTele2))
                .ForMember(e => e.Comment, opt => opt.MapFrom(m => m.ConsumerComments.Length < 225 ? m.ConsumerComments : m.ConsumerComments.Substring(0, 225) ?? null))
                .ForMember(b => b.Postcode, opt => opt.MapFrom(m => m.ConsumerPostCode.NormalisePostcode()));
        }
    }
}
