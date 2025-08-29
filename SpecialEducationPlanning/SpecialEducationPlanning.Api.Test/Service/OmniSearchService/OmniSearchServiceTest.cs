using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SpecialEducationPlanning
.Api.Model.OmniSearchModel;
using SpecialEducationPlanning
.Api.Service.OmniSearch;
using SpecialEducationPlanning
.Business.Model;

using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Service
{
    public class OmniSearchServiceTest : BaseTest
    {
        IOmniSearchService omniSearchService;

        public OmniSearchServiceTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
            omniSearchService = new OmniSearchService();
        }

        private List<BuilderModel> GetTestBuilders()
        {
            var builderModels = new List<BuilderModel>
            {
                new BuilderModel
                {
                    Id = 1,
                    AccountNumber = "1",
                },
                new BuilderModel
                {
                    Id = 2,
                    AccountNumber = "2",
                },
                new BuilderModel
                {
                    Id = 3,
                    AccountNumber = "3",
                },
                new BuilderModel
                {
                    Id = 4,
                    AccountNumber = "4",
                },
                new BuilderModel
                {
                    Id = 5,
                    AccountNumber = "5",
                }
            };
            return builderModels;
        }

        private List<PlanModel> GetTestPlans()
        {
            var preview = Directory.GetCurrentDirectory() + "/Resources/preview-placeholder.jpg";
            var rom = Directory.GetCurrentDirectory() + "/Resources/D0116400001.Rom";
            var planModels = new List<PlanModel>
            {
                new PlanModel
                {
                    Title = "12345",
                    Id = 12345,
                    UpdatedDate = DateTime.UtcNow.AddHours(-3),
                    Versions =
                    {
                        new VersionModel { VersionNotes = "VersionNotes" },
                        new VersionModel { VersionNotes = "VersionNotes" }
                    }
                },
                new PlanModel
                {
                    Title = "12344",
                    Id = 12344,
                    UpdatedDate = DateTime.UtcNow.AddHours(-2),
                    Versions =
                    {
                        new VersionModel {VersionNotes = "VersionNotes"},
                        new VersionModel {VersionNotes = "VersionNotes"}
                    }
                }
            };
            return planModels;
        }

        [Fact]
        public void ListResults()
        {
            var planModels = GetTestPlans();
            var buildModels = GetTestBuilders();
            var results = new List<OmniSearchModel>();
            foreach (var item in buildModels)
            {
                results.Add(new OmniSearchModel
                {
                    Type = typeof(BuilderModel).Name,
                    Object = item,
                    UpdatedDate = item.UpdatedDate
                });
            }
            foreach (var item in planModels)
            {
                results.Add(new OmniSearchModel
                {
                    Type = typeof(PlanModel).Name,
                    Object = item,
                    UpdatedDate = item.UpdatedDate
                });
            }
            var result = results.OrderBy(item => item.UpdatedDate);
            Assert.NotNull(result);
        }
    }
}