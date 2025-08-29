using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpecialEducationPlanning
.Domain.Extensions
{
    public static class NormaliseStrings
    {
        public static string NormaliseNumber(this string inputString)
        {
            return !string.IsNullOrWhiteSpace(inputString) ? new string(inputString.Where(n => char.IsDigit(n)).ToArray()) : null;
        }

        public static string NormaliseAccountNumber(this string accountNumber)
        {
            return !string.IsNullOrWhiteSpace(accountNumber) ? accountNumber.PadLeft(10, '0') : null;
        }

        /// <summary>
        /// Replacing all spaces for empty and setting to Uppercase.
        /// This is used for TDP DDBB.  Example: postcode like w1D 1Nn to W1D1NN.
        /// </summary>
        /// <param name="postcode"></param>
        /// <returns></returns>
        public static string NormalisePostcode(this string postcode)
        {
            if (postcode == "N/A") return "N/P";
            return !string.IsNullOrWhiteSpace(postcode) ? postcode.Trim().Replace(" ", string.Empty).ToUpper() : "N/P";
        }

        public static string SubstringWrapper(this string inputString, IEnumerable<string> substrings, string stringPrefix, string stringSuffix = null)
        {
            foreach (var substring in substrings)
            {
                if (stringPrefix.IsNotNull())
                {
                    inputString = inputString.Replace(substring, stringPrefix + substring);
                }
                if (stringSuffix.IsNotNull())
                {
                    inputString = inputString.Replace(substring, substring + stringSuffix);
                }

            }
            return inputString;
        }

        public static string NormaliseSpaces(this string inputString)
        {
            inputString = inputString.Trim();
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            return regex.Replace(inputString, " ");
        }

        public static string Replace(this string inputString, IEnumerable<string> stringsToReplace, string replacementString)
        {
            foreach (var stringToReplace in stringsToReplace)
            {
                inputString = inputString.Replace(stringToReplace, replacementString);
            }
            return inputString;
        }
    }
}

