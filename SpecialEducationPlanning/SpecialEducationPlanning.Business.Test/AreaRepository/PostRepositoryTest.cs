//using Koa.Domain.Specification.Search;
//using Koa.Mapping.ObjectMapper.Automapper;
//using Koa.Persistence.EntityRepository;
//using Koa.Persistence.EntityRepository.EntityFrameworkCore;
//using Koa.Template.Business.Model;
//using Koa.Template.Business.Repository;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;
//using Xunit.Abstractions;

//namespace Koa.Template.Business.Test.Post
//{
//    public class PostRepositoryTest : BaseTest
//    {
//        private readonly AutoMapperObjectMapper koaMapper;
//        private readonly PostRepository repository;
//        private readonly Mock<IEntityRepository<int>> entityRepositoryMoq;
//        private readonly Mock<IEfUnitOfWork> unitOfWorkMok;

//        public PostRepositoryTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
//        {
//            this.entityRepositoryMoq = new Mock<IEntityRepository<int>>();
//            this.unitOfWorkMok = new Mock<IEfUnitOfWork>();

//            var config = new AutoMapper.MapperConfiguration(cfg =>
//            {
//                cfg.AddProfile<PostModelProfile>();
//            });
//            var automapper = config.CreateMapper();
//            this.koaMapper = new AutoMapperObjectMapper(automapper);

//            this.repository = new PostRepository(
//                this.LoggerFactory.CreateLogger<PostRepository>(),
//                entityRepositoryMoq.Object,
//                unitOfWorkMok.Object,
//                this.koaMapper,
//                new SpecificationBuilder(this.LoggerFactory.CreateLogger<SpecificationBuilder>())
//           );
//        }


//        [Fact]
//        public void GetAllTest()
//        {
//            //Arrange
//            var entities = new[] {
//                new Domain.Entity.Post()
//                    {
//                        Content = "TestContent"
//                    }
//                };
//            this.entityRepositoryMoq
//               .Setup(x => x.GetAll<Domain.Entity.Post>())
//               .Returns(() =>
//               {
//                   return entities.AsQueryable();
//               });

//            //Act
//            var models = this.repository.GetAll<PostModel>().ToArray();

//            //Assert
//            Assert.NotNull(models);
//            Assert.Equal(entities.Count(), models.Count());
//            Assert.IsAssignableFrom<IEnumerable<PostModel>>(models);

//            Assert.Equal(entities[0].Content, models[0].Content);

//        }
//    }
//}
