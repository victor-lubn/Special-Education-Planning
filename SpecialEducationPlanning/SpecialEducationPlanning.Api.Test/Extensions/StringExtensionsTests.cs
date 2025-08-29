using Shouldly;
using SpecialEducationPlanning
.Api.Extensions;

using Xunit;
using Xunit.Abstractions;

namespace SpecialEducationPlanning
.Api.Test.Extensions
{
    public class StringExtensionsTests : BaseTest
    {
        private readonly string _contains_no_html = "string contains no html";
        private readonly string _contains_open_html = "string contains <b> open html";
        private readonly string _contains_closed_html = "string contains <b> closed html </b>";

        private readonly string _contains_open_invalid_html = "string contains <invalid> open html";
        private readonly string _contains_closed_invalid_html = "string contains <invalid> closed html </invalid>";

        private readonly string _contains_null = null;
        private readonly string _contains_empty = string.Empty;

        public StringExtensionsTests(CompositionRootFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {
        }

        [Fact]
        public void Contains_Null_String_Check_For_Open_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_null.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_Empty_String_Check_For_Open_HtmlShould_Return_False()
        {
            //Act
            bool result = _contains_empty.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_Null_String_Check_For_Closed_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_null.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_Empty_String_Check_For_Closed_HtmlShould_Return_False()
        {
            //Act
            bool result = _contains_empty.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_Closed_Html_Should_Return_True()
        {
            //Act
            bool result = _contains_closed_html.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Closed_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_closed_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Open_Html_Should_Return_True()
        {
            //Act
            bool result = _contains_open_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Open_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_open_html.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_No_Open_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_no_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_No_Closed_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_no_html.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_No_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_no_html.ContainsClosedHTMLElements() && _contains_no_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_Open_Html_Test_For_Open_And_Closed_Html_Should_Return_True()
        {
            //Act
            bool result = _contains_open_html.ContainsClosedHTMLElements() || _contains_open_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Closed_Html_Test_For_Open_And_Closed_Html_Should_Return_True()
        {
            //Act
            bool result = _contains_closed_html.ContainsClosedHTMLElements() || _contains_closed_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Invalid_Open_Html_Should_Return_True()
        {
            //Act
            bool result = _contains_open_invalid_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Invalid_Open_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_open_invalid_html.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Contains_Invalid_Closed_Html_Should_Return_False()
        {
            //Act
            bool result = _contains_closed_invalid_html.ContainsOpenHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Contains_Invalid_Closed_Html_Should_Return_True()
        {
            //Act
            bool result = _contains_closed_invalid_html.ContainsClosedHTMLElements();

            //Assert
            result.ShouldBeTrue();
        }
    }
}
