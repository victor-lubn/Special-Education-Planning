using System;
using System.Text;
using SpecialEducationPlanning
.Domain.Entity;

namespace SpecialEducationPlanning
.Domain.Extensions

{
    internal static class EndUserCalculatedProperties
    {




        public static void CalculateProperties(this EndUser endUser)
        {
            endUser.Address0 = ComposeAddress0(endUser);
            endUser.FullName = ComposeFullName(endUser);
        }


        private static string ComposeAddress0(EndUser endUser)
        {
            var stringBuilder = new StringBuilder();
            if (!endUser.Address1.IsNullOrEmpty())
            {
                stringBuilder.Append(endUser.Address1);
            }
            if (!endUser.Address2.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(endUser.Address2);
            }
            if (!endUser.Address3.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(endUser.Address3);
            }
            if (!endUser.Address4.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(endUser.Address4);
            }
            if (!endUser.Address5.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(endUser.Address5);
            }

            return stringBuilder.ToString();
        }

        private static string ComposeFullName(EndUser endUser)
        {
            var stringBuilder = new StringBuilder();
            if (!endUser.FirstName.IsNullOrEmpty())
            {
                stringBuilder.Append(endUser.FirstName);
            }
            if (!endUser.Surname.IsNullOrEmpty())
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(" ");
                stringBuilder.Append(endUser.Surname);
            }

            return stringBuilder.ToString();
        }

    }
}
