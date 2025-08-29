using System.Collections.Generic;

namespace SpecialEducationPlanning
.Api.Model
{
    public class ProblemDetailsModel
    {
        public string Type => $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{StatusCode}";
        public string Title { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public IList<ErrorIssue> Issues { get; set; }

        public ProblemDetailsModel(string title, int statusCode, string message, IList<ErrorIssue> issues = null)
        {
            StatusCode = statusCode;
            Title = title;
            Message = message;
            Issues = issues;
        }

        public ProblemDetailsModel()
        {
        }
    }
}
