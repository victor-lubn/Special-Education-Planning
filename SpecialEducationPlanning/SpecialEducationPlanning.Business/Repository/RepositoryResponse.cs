using System;
using System.Collections.Generic;
using System.Linq;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Business.Repository
{
    public class RepositoryResponse<T>
    {
        public RepositoryResponse(T content, ICollection<string> errorList)
        {
            this.Content = content;
            this.ErrorList = errorList;
        }

        public RepositoryResponse(ICollection<string> errorList)
        {
            ErrorList = errorList;
        }

        public RepositoryResponse(T content)
        {
            Content = content;
        }

        public RepositoryResponse(string error)
        {
            ErrorList = new List<string> { error };
        }

        public RepositoryResponse()
        {
        }
        public bool HasError() { return ErrorList.Any(); }

        public ICollection<string> ErrorList { get; set; } = new List<string>();
        public T Content { get; set; }



        public RepositoryResponse(T content, ErrorCode errorCode, string message = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            this.Content = content;
            this.ErrorList.Add($"{errorCode.GetDescription()} {message} from {callerMemberName}");

        }

        public void AddError(ErrorCode errorCode, string message = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            this.ErrorList.Add($"{errorCode.GetDescription()} {message} from {callerMemberName}");

        }


    }
}