using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpecialEducationPlanning
.Api.Configuration.PostCode;
using SpecialEducationPlanning
.Business.IService;
using SpecialEducationPlanning
.Business.Strategy.PostCode;
using SpecialEducationPlanning
.Domain.Enum;

namespace SpecialEducationPlanning
.Api.Service.PostCode
{
    public class PostCodeServiceFactory: IPostCodeServiceFactory
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<PostCodeService> logger;
        private readonly IOptions<List<PostCodeConfiguration>> options;
        private readonly IPostCodeStrategy postCodeStrategy;


        public PostCodeServiceFactory(IOptions<List<PostCodeConfiguration>> options,
            ILogger<PostCodeService> logger, IPostCodeStrategy postCodeStrategy, HttpClient httpClient)
        {
            this.options = options;
            this.logger = logger;
            this.postCodeStrategy = postCodeStrategy;
            this.httpClient = httpClient;
        }


        public IPostCodeService GetService(string countryCode)
        {
            var filter = new Func<PostCodeConfiguration, bool>(pc => pc.CountryCode.ToLower().Equals(countryCode.ToLower()));

            if (string.IsNullOrEmpty(countryCode)) filter = pc => true;

            return new PostCodeService(options.Value.First(filter), logger, postCodeStrategy, httpClient);
        }
    }
}