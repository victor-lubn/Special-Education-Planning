using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using SpecialEducationPlanning
.Business.Repository;

namespace SpecialEducationPlanning
.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RepositoryResponseExtension
    {
        /// <summary>
        ///     RepositoryResponse extension method for returning HTTP response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repositoryResponse"></param>
        /// <returns></returns>
        public static IActionResult GetHttpResponse<T>(this RepositoryResponse<T> repositoryResponse)
        {
            if (repositoryResponse.ErrorList.Any())
                return new BadRequestObjectResult(repositoryResponse.ErrorList);
            if (repositoryResponse.Content.IsNotNull())
                return new OkObjectResult(repositoryResponse.Content);
            return new OkResult();
        }
    }
}
