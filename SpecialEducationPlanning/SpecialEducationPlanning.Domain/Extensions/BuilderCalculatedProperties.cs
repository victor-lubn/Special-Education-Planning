using System;
using System.Text;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Extensions

{
    internal static class BuilderCalculatedProperties
    {
        public static void Normalise(this Builder builder)
        {
            builder.Address0 = ComposeAddress0(builder);
            builder.MobileNumber = builder.MobileNumber.NormaliseNumber();
            builder.LandLineNumber = builder.LandLineNumber.NormaliseNumber();
            builder.Postcode = builder.Postcode.NormalisePostcode();
            builder.AccountNumber = builder.AccountNumber.NormaliseAccountNumber();
        }


        private static string ComposeAddress0(Builder builder)
        {
            var stringBuilder = new StringBuilder();
            if (!builder.Address1.IsNullOrEmpty())
            {
                stringBuilder.Append(builder.Address1);
            }
            if (!builder.Address2.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(builder.Address2);
            }
            if (!builder.Address3.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(builder.Address3);
            }

            return stringBuilder.ToString();
        }
    }
}
