/*
using SpecialEducationPlanning
.Api.Controllers;
using SpecialEducationPlanning
.Api.WebTest;
using SpecialEducationPlanning
.Business.Model;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test
{
    public class PostControllerTest : BaseTest
    {

        public PostControllerTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

        }

        [Fact]
        [ResetDatabase]
        public async Task UnauthorizedPostRequestWhenAnonymous()
        {
            //Arrange
            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.GetAsync(CancellationToken.None));

            // Act
            var response = await request.GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }

        [Fact]
        [ResetDatabase]
        public async Task SuccessfulPostGet()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            var post = await this.WithPostInTheDatabase(user);

            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.GetAsync(CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);

            var json = await responseMessage.Content.ReadAsStringAsync();
            var responseModels = JsonConvert.DeserializeObject<PostModel[]>(json);

            Assert.Single(responseModels);
            Assert.Equal(post.Id, responseModels[0].Id);

        }

        [Fact]
        [ResetDatabase]
        public async Task SuccessfulSinglePostGetById()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            _ = await this.WithPostInTheDatabase(user);
            var post = await this.WithPostInTheDatabase(user);

            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.GetAsync(post.Id, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);

            var json = await responseMessage.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<PostModel>(json);
            Assert.Equal(post.Id, responseModel.Id);

        }


        [Fact]
        [ResetDatabase]
        public async Task NotFoundPostGetById()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            var post = await this.WithPostInTheDatabase(user);

            int idNotExistent = int.MinValue;

            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.GetAsync(idNotExistent, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);

        }

        [Fact]
        [ResetDatabase]
        public async Task SuccessfulPostCreation()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();

            var postModel = new PostModel
            {
                Content = "TestContent",
                UserId = user.Id
            };
            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.PostAsync(postModel, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.PostAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Created, responseMessage.StatusCode);
            Assert.NotNull(responseMessage.Headers.Location);

            var json = await responseMessage.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<PostModel>(json);

            Assert.True(responseModel.Id > 0);
            Assert.Equal(postModel.Content, responseModel.Content);
            Assert.Equal(postModel.UserId, responseModel.UserId);

        }


        [Fact]
        [ResetDatabase]
        public async Task SuccessfulPostUpdate()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            var post = await this.WithPostInTheDatabase(user);

            string modifiedContent = post.Content + "TestContentModified";

            var postModel = new PostModel
            {
                Id = post.Id,
                Content = modifiedContent,
                UserId = user.Id
            };
            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.PutAsync(postModel.Id, postModel, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.SendAsync(HttpMethod.Put.Method);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);


            var json = await responseMessage.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<PostModel>(json);

            Assert.Equal(postModel.Id, responseModel.Id);
            Assert.Equal(modifiedContent, responseModel.Content);
            Assert.Equal(postModel.UserId, responseModel.UserId);

        }


        [Fact]
        [ResetDatabase]
        public async Task NotFoundPostUpdate()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            var post = await this.WithPostInTheDatabase(user);

            string modifiedContent = post.Content + "TestContentModified";
            int idNotExistent = int.MinValue;

            var postModel = new PostModel
            {
                Id = idNotExistent,
                Content = modifiedContent,
                UserId = user.Id
            };
            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.PutAsync(postModel.Id, postModel, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.SendAsync(HttpMethod.Put.Method);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);


        }

        [Fact]
        [ResetDatabase]
        public async Task SuccessfulPostDelete()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            var post = await this.WithPostInTheDatabase(user);

            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.DeleteAsync(post.Id, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            var requestCheck = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.GetAsync(CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.SendAsync(HttpMethod.Delete.Method);
            var responseRep = await requestCheck.GetAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);

            Assert.Equal(HttpStatusCode.OK, responseRep.StatusCode);
            var json = await responseRep.Content.ReadAsStringAsync();
            var responseModels = JsonConvert.DeserializeObject<PostModel[]>(json);
            Assert.Empty(responseModels);
        }


        [Fact]
        [ResetDatabase]
        public async Task NotFoundPostDelete()
        {
            //Arrange
            var user = await this.WithUserInTheDatabase();
            var post = await this.WithPostInTheDatabase(user);
            int idNotExistent = int.MinValue;

            var request = this.Server
                                .CreateHttpApiRequest<PostController>(controller => controller.DeleteAsync(idNotExistent, CancellationToken.None))
                                .WithIdentity(Identities.TestUser);

            // Act
            var responseMessage = await request.SendAsync(HttpMethod.Delete.Method);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }
    }
}
*/