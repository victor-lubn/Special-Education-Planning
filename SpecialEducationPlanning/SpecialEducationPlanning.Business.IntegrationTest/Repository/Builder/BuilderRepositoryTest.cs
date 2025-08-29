using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Moq;
using SpecialEducationPlanning
.Business.DtoModel;
using SpecialEducationPlanning
.Business.Mapper;
using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;
using SpecialEducationPlanning
.Domain.Enum;
using Xunit;
using Xunit.Abstractions;


namespace SpecialEducationPlanning
.Business.Test.Builder
{
    public class BuilderRepositoryTest : BaseTest
    {
        public BuilderRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(UserModelProfile));
            });
            var mapper = new Mock<AutoMapperObjectMapper>(config.CreateMapper());

           

            repository = fixture.ServiceProvider.GetRequiredService<IBuilderRepository>();
        }

        private readonly IBuilderRepository repository;

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(repository);
        }

        [Fact(Skip = "DATACONTEXT_PROBLEM")]
        public async Task CreateBuilderTest()
        {
            var builder = new BuilderModel
            {
                Id = 99,
                Address1 = "La Moncloa",
                Address2 = "Buckingham Palace",
                Address3 = "White House",
                MobileNumber = "134-121-11212",
                AccountNumber = "464646",
                Postcode = "1233545",
                Name = "John",
                TradingName = "McGregror",
                Email = "myemail@everis.com"
            };

            var Educationer = await CreatesEducationer();
            var Aiep = await CreatesAiep();

            var result = await repository.CreateAsync(builder, Educationer.Id, Aiep.Id);
            Assert.NotNull(result);
            Assert.Equal(builder.TradingName, result.Content.TradingName);
        }

        private async Task<Domain.Entity.User> CreatesEducationer()
        {
            var Educationer = new Domain.Entity.User
            {
                Id = 1,
                FirstName = "John",
                Surname = "Papa"
            };
            var EducationerRepo = Fixture.ServiceProvider.GetService<IEducationerRepository>();
            var createdEducationer = await EducationerRepo.Add(Educationer);
            return createdEducationer;
        }

        private async Task<Domain.Entity.Aiep> CreatesAiep()
        {
            var Aiep = new Domain.Entity.Aiep
            {
                Id = 3
            };
            var AiepRepo = Fixture.ServiceProvider.GetService<IAiepRepository>();
            var createdAiep = await AiepRepo.ApplyChangesAsync(Aiep);

            return createdAiep;
        }

        /*[Fact]
        public async Task BuilderEndUserRefreshAsyncTest()
        {
            var endUser = new EndUserModel
            {
                FirstName = "John",
                Surname = "Mc Gregor"
            };
            var endUserRepo = Fixture.ServiceProvider.GetService<IEndUserRepository>();
            endUser = endUserRepo.Add(endUser);

            RepositoryResponse<BuilderModel> repositoryResponse = new RepositoryResponse<BuilderModel>();
            repositoryResponse.Content = new BuilderModel();
            repositoryResponse.Content.EndUsers.Add(endUser);
            repositoryResponse.Content = await this.repository.ApplyChangesAsync(repositoryResponse.Content);

            repositoryResponse = await this.repository.EndUserRefreshAsync(endUser.Id);
            Assert.Equal(repositoryResponse.Content.EndUserNames, endUser.FirstName + " " + endUser.Surname);
        }*/

        [Fact]
        public async Task ValidateCashBuilderWhithOneTelephoneTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "1234",
                LandLineNumber = "12134242",
                MobileNumber = ""
            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.False(validationResult.ErrorList.Any());
        }

        [Fact]
        public async Task ValidateCashBuilderWhithTwoTelephonesTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "1234",
                LandLineNumber = "242343",
                MobileNumber = "2424242"
            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.False(validationResult.ErrorList.Any());
        }

        [Fact]
        public async Task ValidateCashBuilderWithAccountNumberTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "ACCOUNT-12123-43434"
            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.NotNull(validationResult);
        }

        [Fact]
        public async Task ValidateCashBuilderWithAccountTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "232323",
                MobileNumber = "1212"
            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.False(validationResult.ErrorList.Any());
        }

        [Fact]
        public async Task ValidateCashBuilderWithoutAccountTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "",
                MobileNumber = "1212",
                Postcode = "WER 123",
                Address1 = "1",
                TradingName = "Wayne"

            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.False(validationResult.ErrorList.Any());
        }

        [Fact]
        public async Task ValidateCreditBuilderWithAccountTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "ACT-201210",
                MobileNumber = "1212"
            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.False(validationResult.ErrorList.Any());
        }

        [Fact]
        public async Task ValidateCreditBuilderWithoutAccountTestAsync()
        {
            var builderModel = new BuilderModel
            {
                AccountNumber = "",
                MobileNumber = "1212"
            };
            var validationResult = await repository.IsValidBuilder(builderModel);
            Assert.NotNull(validationResult);
        }

        [Fact]
        public async Task GetExistingBuilderAsyncMatchTest()
        {
            var builder = CreateExistingBuilder();
            repository.Add(builder);

            var builderModel = new BuilderModel
            {
                Id = 3,
                TradingName = "McGregor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "123",
                Address1 = "Rd Holland"
            };
            var existingBuilder = await repository.GetExistingBuilderAsync(builderModel);
            Assert.NotNull(existingBuilder);
        }

        private Domain.Entity.Builder CreateExistingBuilder()
        {
            return new Domain.Entity.Builder
            {
                Id = 3,
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "123",
                Address1 = "Rd Holland",
                TradingName = "Test Name"
            };
        }

        [Fact]
        public async Task ValidateEducationerAiepFailureTest()
        {
            var validation = await repository.ValidateEducationerAiep(1, 1);
            Assert.NotNull(validation);
            Assert.NotEmpty(validation.ErrorList);
        }

        [Fact(Skip = "DATACONTEXT_PROBLEM")]
        public async Task ValidateEducationerAiepTest()
        {
            var Educationer = await CreatesEducationer();
            var Aiep = await CreatesAiep();
            var validation = await repository.ValidateEducationerAiep(1, 3);
            Assert.NotNull(validation);
            Assert.Empty(validation.ErrorList);
        }

        [Fact]
        public async Task GetBuildersOmniSearchTestEmptySearchAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            repository.Add(builder);

            var result = await repository.GetBuildersOmniSearch("",  It.IsAny<int>(),It.IsAny<int>()) as RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>;
            
            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.NotNull(result.Content.Item1);
            Assert.True(result.Content.Item1.Count() == 0);
            //Assert.Single(result.Content.Item1);
        }

        [Fact]
        public async Task GetBuildersOmniSearchTestAsync() {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            repository.Add(builder);

            var result = await repository.GetBuildersOmniSearch("2423444", It.IsAny<int>(), It.IsAny<int>()) as RepositoryResponse<Tuple<IEnumerable<BuilderModel>, int>>;
            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.NotNull(result.Content.Item1);
            //Assert.Single(result.Content.Item1);
        }

        [Fact]
        public async Task ExactGetPosibleTdpMatchingBuildersTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            repository.Add(builder);

            var builderModel = new BuilderModel
            {
                TradingName = "McGregor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "WAR EAL ",
                Address1 = "Rd Holland"
            };

            var result = await repository.GetPosibleTdpMatchingBuilders(builderModel) as RepositoryResponse<ValidationBuilderModel>;
            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.NotNull(result.Content.Builders);
            //Assert.Single(result.Content.Builders);
        }

        [Fact]
        public async Task NotExactGetPosibleTdpMatchingBuildersTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            repository.Add(builder);

            Domain.Entity.Builder builder2 = CreateSearchBuilder2();
            repository.Add(builder2);

            var builderModel = new BuilderModel
            {
                TradingName = "McConnor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "WAR EAL ",
                Address1 = "Rd Holland"
            };

            var result = await repository.GetPosibleTdpMatchingBuilders(builderModel) as RepositoryResponse<ValidationBuilderModel>;
            Assert.NotNull(result);
            Assert.NotNull(result.Content);
            Assert.NotNull(result.Content.Builders);
            //Assert.Equal(2, result.Content.Builders.Count());
        }

        [Fact]
        public async Task EmptyGetPosibleTdpMatchingBuildersTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            repository.Add(builder);

            Domain.Entity.Builder builder2 = CreateSearchBuilder2();
            repository.Add(builder2);

            var builderModel = new BuilderModel
            {
                TradingName = "McConnor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "1234",
                Address1 = "Rd Holland"
            };

            var result = await repository.GetPosibleTdpMatchingBuilders(builderModel) as RepositoryResponse<ValidationBuilderModel>;
            Assert.Empty(result.Content.Builders);
        }

        [Fact]
        public async Task GetPosibleBuildersMatchTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            repository.Add(builder);

            var builderModel = new BuilderModel
            {
                Id = 3,
                TradingName = "McGregor",
                Name = "Michael",
                AccountNumber = "1111",
                MobileNumber = "121345",
                Postcode = "WAR EAL ",
                Address1 = "Rd Holland"
            };

            var result = await repository.GetPosibleBuildersMatch(builderModel) as RepositoryResponse<IEnumerable<BuilderModel>>;
            Assert.NotNull(result.Content);
        }

        [Fact(Skip = "DATACONTEXT_PROBLEM")]
        public async Task UpdateBuildersFromSapTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            Domain.Entity.Builder builder2 = CreateSearchBuilder2();
            builder.Id = 1;
            builder2.Id = 2;

            repository.Add(builder);
            repository.Add(builder2);

            var builderModel = CreateSearchBuilderSapModel();
            var builderModel2 = CreateSearchBuilderSapModel2();

            builderModel.Address2 = "myhome";
            builderModel2.Address2 = "myhome";

            var list = new List<BuilderSapModel>() { builderModel, builderModel2 };

            var result = await repository.UpdateBuildersFromSapAsync(list);
            Assert.NotEmpty(result.Content);
        }

        [Fact(Skip = "DATACONTEXT_PROBLEM")]
        public async Task UpdateBuildersFromSapAccountNumberTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilderAccountNumber();
            Domain.Entity.Builder builder2 = CreateSearchBuilderAccountNumber2();
            builder.Id = 1;
            builder2.Id = 2;

            repository.Add(builder);
            repository.Add(builder2);

            var builderModel = CreateSearchBuilderSapModelAccountNumber();
            var builderModel2 = CreateSearchBuilderSapModelAccountNumber2();

            builderModel.Address2 = "myhome";
            builderModel2.Address2 = "myhome";

            var list = new List<BuilderSapModel>() { builderModel, builderModel2 };

            var result = await repository.UpdateBuildersFromSapAsync(list);
            Assert.NotEmpty(result.Content);
        }

        [Fact]
        public async Task UpdateBuildersFromSapNotFoundTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilder();
            Domain.Entity.Builder builder2 = CreateSearchBuilder2();
            builder.Id = 1;
            builder2.Id = 2;

            repository.Add(builder);
            repository.Add(builder2);

            var builderModel = CreateSearchBuilderSapModel();
            var builderModel2 = CreateSearchBuilderSapModel2();

            builderModel.Address1 = "myhome";
            builderModel2.Address1 = "myhome";

            var list = new List<BuilderSapModel>() { builderModel, builderModel2 };

            var result = await repository.UpdateBuildersFromSapAsync(list);
            Assert.NotEmpty(result.ErrorList);
        }

        [Fact]
        public async Task UpdateBuildersFromSapNotFoundAccountNumberTestAsync()
        {
            Domain.Entity.Builder builder = CreateSearchBuilderAccountNumber();
            Domain.Entity.Builder builder2 = CreateSearchBuilderAccountNumber2();
            builder.Id = 1;
            builder2.Id = 2;

            repository.Add(builder);
            repository.Add(builder2);

            var builderModel = CreateSearchBuilderSapModelAccountNumber();
            var builderModel2 = CreateSearchBuilderSapModelAccountNumber2();

            builderModel.AccountNumber = "678";
            builderModel2.AccountNumber = "678";

            var list = new List<BuilderSapModel>() { builderModel, builderModel2 };

            var result = await repository.UpdateBuildersFromSapAsync(list);
            Assert.NotEmpty(result.ErrorList);
        }

        private Domain.Entity.Builder CreateSearchBuilder()
        {
            return new Domain.Entity.Builder
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Address0 = "",
                Address1 = "Rd Holland",
                TradingName = "Test Name"
            };
        }

        private Domain.Entity.Builder CreateSearchBuilderAccountNumber()
        {
            return new Domain.Entity.Builder
            {
                AccountNumber = "123",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Address0 = "",
                Address1 = "Rd Holland",
                TradingName = "Test Name"
            };
        }

        private Domain.Entity.Builder CreateSearchBuilderAccountNumber2()
        {
            return new Domain.Entity.Builder
            {
                AccountNumber = "1234",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Address0 = "",
                Address1 = "Rd Holland",
                TradingName = "Test Name"
            };
        }

        private BuilderModel CreateSearchBuilderModel()
        {
            return new BuilderModel
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Address0 = "",
                Address1 = "Rd Holland",
                TradingName = "McGregor",
                Name = "Michael",
            };
        }

        private Domain.Entity.Builder CreateSearchBuilder2()
        {
            return new Domain.Entity.Builder
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Address0 = "",
                Address1 = "Rd Holland",
                TradingName = "Test Name"
            };
        }

        private BuilderModel CreateSearchBuilderModel2()
        {
            return new BuilderModel
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Email = "",
                Address0 = "",
                Address1 = "Rd Holland",
                TradingName = "McDonald",
                Name = "Michael",
            };
        }

        private BuilderSapModel CreateSearchBuilderSapModel()
        {
            return new BuilderSapModel
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Email = "",
                Address1 = "Rd Holland",
                Name = "McDolan",
                TradingName = "Michael",
                BuilderStatus = BuilderStatus.Active
            };
        }

        private BuilderSapModel CreateSearchBuilderSapModelAccountNumber()
        {
            return new BuilderSapModel
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Email = "",
                Address1 = "Rd Holland",
                Name = "McGregor",
                TradingName = "Michael",
                BuilderStatus = BuilderStatus.Active
            };
        }

        private BuilderSapModel CreateSearchBuilderSapModel2()
        {
            return new BuilderSapModel
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Email = "",
                Address1 = "Rd Holland",
                Name = "McDonald",
                TradingName = "Michael",
                BuilderStatus = BuilderStatus.Active
            };
        }

        private BuilderSapModel CreateSearchBuilderSapModelAccountNumber2()
        {
            return new BuilderSapModel
            {
                AccountNumber = "",
                LandLineNumber = "",
                MobileNumber = "2423444",
                Postcode = "WAR EAL ",
                Email = "",
                Address1 = "Rd Holland",
                Name = "McGregors",
                TradingName = "Michael",
                BuilderStatus = BuilderStatus.Active
            };
        }
    }
}

