using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Api.Model;

namespace SpecialEducationPlanning
.Api.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static IEnumerable<string> GetErrorMessages(this ModelStateDictionary modelState)
        {
            var errorMessageList = modelState.Values.SelectMany(m => m.Errors)
                .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                .Select(e => e.ErrorMessage).ToList();
            var exceptionMessageList = modelState.Values.SelectMany(m => m.Errors).Where(x => x.Exception != null)
                .Select(e => e.Exception.Message).ToList();
            errorMessageList.AddRange(exceptionMessageList);
            return errorMessageList;
        }

        public static IList<ErrorIssue> GetErrorIssues(this ModelStateDictionary modelState)
        {
            var errorMessageList = modelState
                .Where(entry => entry.Value.Errors.Any())
                .SelectMany(entry => entry.Value.Errors.Select(error => new ErrorIssue
                {
                    FieldName = entry.Key,
                    Error = error.ErrorMessage
                }))
                .ToList();

            var exceptionMessageList = modelState
                .Where(entry => entry.Value.Errors.Any(e => e.Exception != null))
                .SelectMany(entry => entry.Value.Errors.Where(e => e.Exception != null)
                    .Select(error => new ErrorIssue
                    {
                        FieldName = entry.Key,
                        Error = error.Exception.Message
                    }))
                .ToList();

            errorMessageList.AddRange(exceptionMessageList);

            return errorMessageList;
        }
    }
}