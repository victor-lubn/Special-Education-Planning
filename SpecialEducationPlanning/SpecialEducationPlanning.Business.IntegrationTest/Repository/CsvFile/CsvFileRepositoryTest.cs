using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using Koa.Persistence.EntityRepository;
using Koa.Persistence.EntityRepository.EntityFrameworkCore;

using SpecialEducationPlanning
.Business.Model;
using SpecialEducationPlanning
.Business.Repository;

using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.Repository.CsvFile
{
    public class CsvFileRepositoryTest : BaseTest
    {
        private readonly ICsvFileRepository Repository;
        private readonly IEntityRepository<int> EntityRep;
        private readonly IEfUnitOfWork UnitOfWork;
        public CsvFileRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            Repository = fixture.ServiceProvider.GetRequiredService<ICsvFileRepository>();
            EntityRep = fixture.ServiceProvider.GetRequiredService<IEntityRepository<int>>();
            UnitOfWork = fixture.ServiceProvider.GetRequiredService<IEfUnitOfWork>();
        }

        [Fact]
        public void CanInjectTest()
        {
            Assert.NotNull(Repository);
        }

        [Fact]
        public void InsertNoAiepUsersTest()
        {
            var role = new Domain.Entity.Role() { Id = 2, Name = "Educationer" };
            EntityRep.Add<Domain.Entity.Role>(role);
            UnitOfWork.Commit();
            var users = GetUserRecordsNoAiep();
            var result = Repository.InsertUsers(users);
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.ErrorList.Count);
        }

        [Fact]
        public void InsertOkAiepTest()
        {
            var area = new Domain.Entity.Area() { Id = 1, KeyName = "London"};
            var Aieps = GetAiepRecords();
            EntityRep.Add<Domain.Entity.Area>(area);
            UnitOfWork.Commit();
            var result = Repository.InsertAieps(Aieps);
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.Content);
        }

        [Fact]
        public void InsertDuplicateAiepTest()
        {
            var area = new Domain.Entity.Area() { Id = 1, KeyName = "London" };            
            var Aieps = GetDuplicateAiepRecords();
            EntityRep.Add<Domain.Entity.Area>(area);
            UnitOfWork.Commit();
            var result = Repository.InsertAieps(Aieps);
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.ErrorList.Count);
        }

        [Fact]
        public void InsertAiepNoAreaTest()
        {
            var area = new Domain.Entity.Area() { Id = 1, KeyName = "London" };            
            EntityRep.Add<Domain.Entity.Area>(area);
            UnitOfWork.Commit();
            var Aieps = GetAiepNoAreaRecords();
            var result = Repository.InsertAieps(Aieps);
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.ErrorList.Count);
        }

        [Fact]
        public void InsertAiepNoDownloadTest()
        {
            var area = new Domain.Entity.Area() { Id = 1, KeyName = "London" };
            var Aieps = GetAiepNoDownloadRecords();
            EntityRep.Add<Domain.Entity.Area>(area);
            UnitOfWork.Commit();
            var result = Repository.InsertAieps(Aieps);
            Assert.NotNull(result);
            Assert.Equal(1, result.Result.ErrorList.Count);
        }
        private IEnumerable<UserCsvModel> GetUserRecords()
        {            
            UserCsvModel model = new UserCsvModel();
            model.userprincipalName = "Billy.NorthWatford@aiep.com";
            model.departmentNumber = "DD86";
            model.role = "Educationer";
            List<UserCsvModel> records = new List<UserCsvModel> { model};
            return records;
        }

        private IEnumerable<UserCsvModel> GetUserRecordsNoAiep()
        {
            UserCsvModel model = new UserCsvModel();
            model.userprincipalName = "Billy.NorthWatford@aiep.com";
            model.role = "Educationer";
            List<UserCsvModel> records = new List<UserCsvModel> { model };
            return records;
        }

        private IEnumerable<AiepCsvModel> GetAiepRecords()
        {
            AiepCsvModel model = new AiepCsvModel();
            model.Area = "London";
            model.AiepCode = "DX99";
            model.DownloadSpeed = "0";
            model.DownloadLimit = "0";
            model.Manager = "manager@aiep.com";
            List<AiepCsvModel> records = new List<AiepCsvModel> { model };
            return records;
        }

        private IEnumerable<AiepCsvModel> GetDuplicateAiepRecords()
        {
            AiepCsvModel model = new AiepCsvModel();
            model.Area= "London";
            model.AiepCode = "DX99";
            model.DownloadSpeed = "0";
            model.DownloadLimit = "0";
            model.Manager = "manager@aiep.com";
            AiepCsvModel model2 = new AiepCsvModel();
            model2.Area = "London";
            model2.AiepCode = "DX99";
            model2.DownloadSpeed = "0";
            model2.DownloadLimit = "0";
            model2.Manager = "manager@aiep.com";
            List<AiepCsvModel> records = new List<AiepCsvModel> { model, model2 };
            return records;
        }

        private IEnumerable<AiepCsvModel> GetAiepNoAreaRecords()
        {
            AiepCsvModel model = new AiepCsvModel();
            model.Area = "No";
            model.AiepCode = "DX99";
            model.DownloadSpeed = "0";
            model.DownloadLimit = "0";
            model.Manager = "manager@aiep.com";
            List<AiepCsvModel> records = new List<AiepCsvModel> { model };
            return records;
        }

        private IEnumerable<AiepCsvModel> GetAiepNoDownloadRecords()
        {
            AiepCsvModel model = new AiepCsvModel();
            model.Area = "London";
            model.AiepCode = "DX99";
            model.Manager = "manager@aiep.com";
            List<AiepCsvModel> records = new List<AiepCsvModel> { model };
            return records;
        }
    }
}


