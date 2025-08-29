using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Domain.Extensions;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{

    /// <summary>
    /// Customer migration model profile
    /// </summary>
    public class CustomerMigrationModelProfile : BaseProfile<CustomerMigrationModel, Builder, int>
    {
        private const string closedSapAccountValue = "X";

        protected override IMappingExpression<Builder, CustomerMigrationModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(m => m.TdpId, o => o.MapFrom(e => e.Id));
        }

        protected override IMappingExpression<CustomerMigrationModel, Builder> MapModelToEntity()
        {
            return base.MapModelToEntity()
                .ForMember(b => b.Id, opt => opt.Ignore())
                .ForMember(b => b.TradingName, opt => opt.MapFrom(m => m.Surname))
                .ForMember(b => b.Address0, opt => opt.MapFrom(m => m.HouseName))
                .ForMember(b => b.Address1, opt => opt.MapFrom(m => new List<string>() { m.HouseName, m.Address1 }.ComposeAddress()))
                .ForMember(b => b.Address2, opt => opt.MapFrom(m => m.Address2))
                .ForMember(b => b.Address3, opt => opt.MapFrom(m => new List<string>() { m.Address3, m.Address4, m.Address5 }.ComposeAddress()))
                .ForMember(b => b.Postcode, opt => opt.MapFrom(m => m.Postcode.NormalisePostcode()))
                .ForMember(b => b.MobileNumber, opt => opt.MapFrom(m => m.PhoneWork))
                .ForMember(b => b.LandLineNumber, opt => opt.MapFrom(m => m.PhoneHome))
                .ForMember(b => b.Email, opt => opt.MapFrom(m => m.Email))
                .ForMember(b => b.CreatedDate, opt => opt.MapFrom(s => DateTime.MaxValue))
                .ForMember(b => b.UpdatedDate, opt => opt.MapFrom(m => m.PlanDate))
                .ForMember(b => b.Notes, opt => opt.MapFrom(m => new List<string>() { m.Email2, m.SalesPerson, m.PlanDate.ToString() }.ComposeAddress()))
                .ForMember(b => b.BuilderStatus, opt => opt.MapFrom(m => (m.BuilderStatus.ToString() == closedSapAccountValue) ? BuilderStatus.Closed : BuilderStatus.Active));
        }
    }

    public static class PostCodeExtensions
    {
        /// <summary>
        /// Given a set of strings, represent a part of an address, composes an Address-like string
        /// </summary>
        /// <param name="value">List of address parts (e.g. House Name, Address 1, Address 2, Postcode...</param>
        /// <returns>An address-like string</returns>
        public static string ComposeAddress(this List<string> value)
        {
            return string.Join(", ", value.Where(x => !string.IsNullOrEmpty(x))).TrimStart(' ', ',').TrimEnd(' ', ',');
        }

        /// <summary>
        /// Transforming to Uppercase and setting the correct spacing in between.
        /// This is used also for SAP.  Example: postcode like w1D 1Nn to W1D 1NN.
        /// </summary>
        /// <param name="ukPostcode"></param>
        /// <returns></returns>
        public static string RepresentUKPostcode(this string ukPostcode)
        {
            ukPostcode = ukPostcode.NormalisePostcode();
            ukPostcode = FormatUKPostcode(ukPostcode);
            return ukPostcode;
        }



        /// <summary>
        /// Setting spacing in between postcode
        /// Example: postcode like w1D1Nn to w1 D1Mm.
        /// </summary>
        /// <param name="ukPostcode"></param>
        /// <returns></returns>
        private static string FormatUKPostcode(this string ukPostcode)
        {
            switch (ukPostcode.Length)
            {
                //add space after 2 characters if length is 5 
                case 5: ukPostcode = ukPostcode.Insert(2, " "); break;
                //add space after 3 characters if length is 6 
                case 6: ukPostcode = ukPostcode.Insert(3, " "); break;
                //add space after 4 characters if length is 7 
                case 7: ukPostcode = ukPostcode.Insert(4, " "); break;

                default: break;
            }

            return ukPostcode;
        }
    }
}
