using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SpecialEducationPlanning
.Business.DtoModel
{

    public class CodeInjectionReject : ValidationAttribute
    {

        private static readonly Regex RegexJavaScript = new Regex(@"<script[^>]*>[\s\S]*?</script>");

        #region Methods Public

        public string GetErrorMessage()
        {
            return "Input can not contain JavaScript code.";
        }

        #endregion

        #region Methods Protected

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (!(value is string inputString))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return RegexJavaScript.IsMatch(inputString)
                ? new ValidationResult(GetErrorMessage())
                : ValidationResult.Success;
        }

        #endregion

    }

}