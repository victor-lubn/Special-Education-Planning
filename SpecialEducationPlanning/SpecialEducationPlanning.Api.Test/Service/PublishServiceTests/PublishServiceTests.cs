using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using SpecialEducationPlanning
.Api.Configuration.PublishSystemService;

using Moq;
using Xunit;
using Xunit.Abstractions;
using Flurl;
using RichardSzalay.MockHttp;
using Newtonsoft.Json;
using Shouldly;

namespace SpecialEducationPlanning
.Api.Test.Service.Publish
{
    public class PublishServiceTests : BaseTest
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<PublishServiceConfiguration>> _mockPublichConfiguration;
        private readonly Guid _jobId;
        private readonly IEnumerable<string> _versionCodes;
        private readonly IEnumerable<string> _versionMultiCodes;
        private const string _baseUrl = "https://app-cadrend-dev-uks-003.azurewebsites.net/api/Orchestrator/";

        //expected results
        private const string _httpWithJobId = "https://app-cadrend-dev-uks-003.azurewebsites.net/api/Orchestrator/PublishJob/JobId/1faf0b6e-197c-435b-9672-a0ae15118ced";
        private const string _httpWithVersionCode = "https://app-cadrend-dev-uks-003.azurewebsites.net/api/Orchestrator/PublishJob/VersionCodes/[\"1\"]";
        private const string _httpWithMultiVersionCode = "https://app-cadrend-dev-uks-003.azurewebsites.net/api/Orchestrator/PublishJob/VersionCodes/[\"1\",\"2\",\"3\"]";

        protected CompositionRootFixture _textfixture;
        protected ITestOutputHelper _outputHelper;

        /// <summary>
        /// Ensure that the nuget package FlUrl returns the same HTTP strings as String.Concat()
        /// when building URL strings
        /// </summary>
        /// <param name="fixture"></param>
        /// <param name="outputHelper"></param>
        public PublishServiceTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            this._textfixture= fixture;
            this._outputHelper= outputHelper;

            this._jobId = Guid.Parse("1faf0b6e-197c-435b-9672-a0ae15118ced");
            this._versionCodes = new Collection<string> { "1" };
            this._versionMultiCodes = new Collection<string> { "1", "2", "3" };

            this._mockPublichConfiguration = new Mock<IOptions<PublishServiceConfiguration>>(MockBehavior.Default);
            PublishServiceConfiguration config = new()
            {
                BaseUrl = PublishServiceTests._baseUrl,
                GetPublishJobsByJobIdUrl = "PublishJob/JobId/",
                GetPublishJobsByVersionCodesUrl = "PublishJob/VersionCodes/",
                HealthCheckUrl = "healthz",
                Token = "d7e6642e8dfd7f623238df8w72df"
            };

            this._mockPublichConfiguration.Setup(x => x.Value).Returns(config);

            //mock the http client so we don't make actual calls to the API
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://app-cadrend-dev-uks-003.azurewebsites.net/api/Orchestrator/*")
                .Respond("application/json", "{'Status' : '200'}"); 
                        
            this._httpClient = mockHttp.ToHttpClient();
            this._httpClient.BaseAddress = new Uri(config.BaseUrl);
            this._httpClient.DefaultRequestHeaders.Add("basic-token", config.Token);
        }

        #region Flurl vs String.Concat() URL builder tests

        [Fact]
        public async Task Flurl_Build_GetPublishJobsByJobIdUrl_Url_Returns_Valid_Response()
        {
            //Arrange
            var config = this._mockPublichConfiguration.Object;
            string url = new Url(config.Value.GetPublishJobsByJobIdUrl).AppendPathSegment(this._jobId);

            //Act
            var httpResponse = await _httpClient.GetAsync(url);

            //Assert
            httpResponse.ShouldNotBeNull();
            httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            httpResponse.IsSuccessStatusCode.ShouldBeTrue();
            httpResponse.RequestMessage!.RequestUri!.AbsoluteUri.ShouldBe(PublishServiceTests._httpWithJobId);
        }

        [Fact]
        public async Task StringConcatl_Build_GetPublishJobsByJobIdUrl_Url_Returns_Valid_Response()
        {
            //Arrange
            var config = this._mockPublichConfiguration.Object;
            string url = String.Concat(config.Value.GetPublishJobsByJobIdUrl, this._jobId);

            //Act
            var httpResponse = await _httpClient.GetAsync(url);

            //Assert
            httpResponse.ShouldNotBeNull();
            httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            httpResponse.IsSuccessStatusCode.ShouldBeTrue();
            httpResponse.RequestMessage!.RequestUri!.AbsoluteUri.ShouldBe(PublishServiceTests._httpWithJobId);
        }

        [Fact]
        public async Task Flurl_Build_GetPublishJobsByVersionCodesUrl_Url_Returns_Valid_Response()
        {
            //Arrange
            var config = this._mockPublichConfiguration.Object;
            var versionCodesSerialised = JsonConvert.SerializeObject(this._versionCodes);
            string url = new Url(config.Value.GetPublishJobsByVersionCodesUrl).AppendPathSegment(versionCodesSerialised);

            //Act
            var httpResponse = await _httpClient.GetAsync(url);

            //Assert
            httpResponse.ShouldNotBeNull();
            httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            httpResponse.IsSuccessStatusCode.ShouldBeTrue();
            httpResponse.RequestMessage!.RequestUri.ShouldBe(new Uri(PublishServiceTests._httpWithVersionCode));
        }

        [Fact]
        public async Task StringConcat_Build_GetPublishJobsByVersionCodesUrl_Url_Returns_Valid_Response()
        {
            //Arrange
            var config = this._mockPublichConfiguration.Object;
            var versionCodesSerialised = JsonConvert.SerializeObject(this._versionCodes);
            string url = String.Concat(config.Value.BaseUrl, config.Value.GetPublishJobsByVersionCodesUrl, versionCodesSerialised);

            //Act
            var httpResponse = await _httpClient.GetAsync(url);

            //Assert
            httpResponse.ShouldNotBeNull();
            httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            httpResponse.IsSuccessStatusCode.ShouldBeTrue();
            httpResponse.RequestMessage!.RequestUri.ShouldBe(new Uri(PublishServiceTests._httpWithVersionCode));
        }

        [Fact]
        public async Task Flurl_Build_GetPublishJobsByVersionCodesUrl_Multi_Url_Returns_Valid_Response()
        {
            //Arrange
            var config = this._mockPublichConfiguration.Object;
            var versionCodesSerialised = JsonConvert.SerializeObject(this._versionMultiCodes);
            string url = new Url(config.Value.GetPublishJobsByVersionCodesUrl).AppendPathSegment(versionCodesSerialised);

            //Act
            var httpResponse = await _httpClient.GetAsync(url);

            //Assert
            httpResponse.ShouldNotBeNull();
            httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            httpResponse.IsSuccessStatusCode.ShouldBeTrue();
            httpResponse.RequestMessage!.RequestUri!.ShouldBe(new Uri(PublishServiceTests._httpWithMultiVersionCode));
        }

        [Fact]
        public async Task StringConcat_Build_GetPublishJobsByVersionCodesUrl_Multi_Url_Returns_Valid_Response()
        {
            //Arrange
            var config = this._mockPublichConfiguration.Object;
            var versionCodesSerialised = JsonConvert.SerializeObject(this._versionMultiCodes);
            string url = String.Concat(config.Value.BaseUrl, config.Value.GetPublishJobsByVersionCodesUrl, versionCodesSerialised);

            //Act
            var httpResponse = await _httpClient.GetAsync(url);

            //Assert
            httpResponse.ShouldNotBeNull();
            httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            httpResponse.IsSuccessStatusCode.ShouldBeTrue();
            httpResponse.RequestMessage!.RequestUri.ShouldBe(new Uri(PublishServiceTests._httpWithMultiVersionCode));
        }

        #endregion
    }
}
