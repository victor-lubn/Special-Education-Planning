using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.DataMigration
{
    public class ZipTest : BaseTest
    {
        public ZipTest(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
        }

        [Fact]
        public void UnzipPlan()
        {
            var basePath = Path.Combine(Environment.CurrentDirectory, "DataMigration");
            var zipUtil = new CadFileUnzip();
            var file = File.ReadAllBytes(Path.Combine(basePath, "plan.zip"));
            var unzip = zipUtil.UnzipPlan(file);
            File.WriteAllBytes(Path.Combine(basePath, "plan.rom"), unzip);
            Assert.True(unzip.Length > 0);
        }


        [Fact]
        public void UnzipPreview()
        {
            var basePath = Path.Combine(Environment.CurrentDirectory, "DataMigration");
            var zipUtil = new CadFileUnzip();
            var file = File.ReadAllBytes(Path.Combine(basePath, "preview.zip"));
            var unzip = zipUtil.UnzipPreview(file);
            File.WriteAllBytes(Path.Combine(basePath, "preview.jpeg"), unzip);
            Assert.True(unzip.Length > 0);
        }
    }
}
