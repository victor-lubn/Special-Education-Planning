using System;
using System.Text.RegularExpressions;

namespace SpecialEducationPlanning
.Api.Extensions
{
    public static class StringExtensions
    {
        //The following will match any matching set of tags. i.e. <b>this</b>
        public static bool ContainsClosedHTMLElements(this string text)
        {
            if (text.IsNullOrEmpty()) return false;
            Regex tagRegex = new (@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            return tagRegex.IsMatch(text);
        }

        //The following will match any single tag. i.e. <b> (it doesn't have to be closed).
        public static bool ContainsOpenHTMLElements(this string text)
        {
            if (text.IsNullOrEmpty()) return false;

            Regex tagRegex = new (@"<[^>]+>");
            return tagRegex.IsMatch(text);
        }
    }
}
