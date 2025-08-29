using AutoMapper;
using System;
using System.Linq;
using SpecialEducationPlanning
.Domain.Entity;
using SpecialEducationPlanning
.Domain.Enum;
using SpecialEducationPlanning
.Business.Mapper;


namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{

    /// <summary>
    ///     Quote migration model profile
    /// </summary>
    public class QuoteMigrationModelProfile : BaseProfile<QuoteMigrationModel, Plan, int>
    {

        #region Methods Public

        public static string GenerateCadFileId(string AiepFile, int customerId, int quoteNumber)
        {
            var AiepNum = LastCharacters(AiepFile, 3);
            var customerIdString = NumberToChar(customerId);
            var quoteNumberString = NumberToChar(quoteNumber);

            return string.Concat(AiepNum, customerIdString, quoteNumberString);
        }

        #endregion

        #region Methods Protected

        protected override IMappingExpression<Plan, QuoteMigrationModel> MapEntityToModel()
        {
            return base.MapEntityToModel().ForMember(m => m.TdpVersionId,
                    o => o.MapFrom(e => e.Versions.FirstOrDefault() == null ? -1 : e.Versions.FirstOrDefault().Id))
                .ForMember(m => m.TdpId, o => o.MapFrom(e => e.Id));
        }

        protected override IMappingExpression<QuoteMigrationModel, Plan> MapModelToEntity()
        {
            return base.MapModelToEntity().ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.Survey, opt => opt.MapFrom(m => m.Survey))

                // PlanCode generated in the service
                //.ForMember(e => e.PlanCode, opt => opt.MapFrom(m => m.QuoteNmbr))
                .ForMember(e => e.CreatedDate, opt => opt.MapFrom(s => DateTime.MaxValue))
                .ForMember(e => e.UpdatedDate, opt => opt.MapFrom(m => m.PlanDate))
                .ForMember(e => e.Educationer, opt => opt.Ignore())
                .ForMember(e => e.PlanType, opt => opt.MapFrom(m => m.ConsumerPlanType))
                .ForMember(e => e.CadFilePlanId,
                    opt => opt.MapFrom(m => GenerateCadFileId(m.AiepFile, m.CustomerId, m.QuoteNmbr)))
                .ForMember(e => e.Survey, opt => opt.MapFrom(m => m.Survey ?? false))
                .ForMember(e => e.BuilderId, opt => opt.MapFrom(m => m.CustomerTdpId))

                // If Entity Framework interceptor finds MaxValue will allow us to modify UpdateDate.
                // Change requested to send UpdatedDate from CADFile for archiving purposes
                // DON'T CHANGE
                .ForMember(e => e.CreatedDate, opt => opt.MapFrom(s => DateTime.MaxValue))

                // As per mapping analysis and commented in Exodus, PlanDate contains CADFile UpdateDate
                .ForMember(e => e.UpdatedDate, opt => opt.MapFrom(m => m.PlanDate ?? DateTime.MinValue))
                .ForMember(e => e.PlanState, opt => opt.MapFrom(s => PlanState.Active));
        }

        #endregion

        #region Methods Private

        private static string LastCharacters(object value, int digits)
        {
            if (value == null)
            {
                return null;
            }

            var stringValue = value.ToString();
            var difference = stringValue.Length - digits;

            if (difference == 0)
            {
                return stringValue;
            }

            if (difference < 0)
            {
                return stringValue.PadLeft(digits, '0');
            }

            return stringValue.Substring(difference);
        }

        private static string NumberToChar(int number)
        {
            //CadFile PlanId legacy logic

            //A + 35 = Z; Maximum value in CadFile is 35999
            if (number >= 36000)
            {
                throw new NotSupportedException("customerId or quoteNumber > 36000");
            }

            var stringNumber = number.ToString("0000");

            //A -1 = ?; return original value
            if (number < 10000)
            {
                return stringNumber;
            }

            var charNumber = (int)'A';
            var increment = number / 1000 - 10;
            var prefix = (char)(charNumber + increment);

            return string.Concat(prefix, stringNumber.Substring(stringNumber.Length - 3));
        }

        #endregion

    }

}

