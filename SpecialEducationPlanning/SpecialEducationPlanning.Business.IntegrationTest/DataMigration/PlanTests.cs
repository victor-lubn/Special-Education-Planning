using System;
using System.Collections.Generic;
using System.Text;
using SpecialEducationPlanning
.Business.Model.DataMigrationModel;
using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Business.Test.DataMigration
{
    public class PlanTests : BaseTest
    {
        public PlanTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
        }

        [Fact]
        public void CadFilePlanIdGeneration1Digit()
        {
            var AiepCode = "D238";
            var customerId = 1;
            var quoteNumber = 1;
            var expected = "23800010001";
            var actual = QuoteMigrationModelProfile.GenerateCadFileId(AiepCode, customerId, quoteNumber);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CadFilePlanIdGeneration2Digits()
        {
            var AiepCode = "D238";
            var customerId = 12;
            var quoteNumber = 10;
            var expected = "23800120010";
            var actual = QuoteMigrationModelProfile.GenerateCadFileId(AiepCode, customerId, quoteNumber);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CadFilePlanIdGeneration3Digits()
        {
            var AiepCode = "D999";
            var customerId = 186;
            var quoteNumber = 122;
            var expected = "99901860122";
            var actual = QuoteMigrationModelProfile.GenerateCadFileId(AiepCode, customerId, quoteNumber);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CadFilePlanIdGeneration4Digits()
        {
            var AiepCode = "D260";
            var customerId = 6000;
            var quoteNumber = 1;
            var expected = "26060000001";
            var actual = QuoteMigrationModelProfile.GenerateCadFileId(AiepCode, customerId, quoteNumber);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CadFilePlanIdGeneration5Digits()
        {
            var AiepCode = "D999";
            var customerId = 18628;
            var quoteNumber = 12383;
            var expected = "999I628C383";
            var actual = QuoteMigrationModelProfile.GenerateCadFileId(AiepCode, customerId, quoteNumber);
            Assert.Equal(expected, actual);
        }
    }
}

