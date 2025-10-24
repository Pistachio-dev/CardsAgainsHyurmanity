using CardsAgainstHyurmanity.Modules.WhiteCardFitting;
using DalamudBasics.Logging;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.CombinedCards
{
    public class CombinedCardFittingTests
    {
        [Theory]
        [InlineData("This is already perfect", "This is already perfect")]
        [InlineData("we got a a duplicate", "We got a duplicate")]
        [InlineData("we got a A duplicate", "We got a duplicate")]
        [InlineData("we got a tHe duplicate", "We got a duplicate")]
        [InlineData("we got thE the duplicate", "We got the duplicate")]
        [InlineData("we got the A duplicate", "We got the duplicate")]
        [InlineData("do capitalize this","Do capitalize this")]
        [InlineData("Get this marker out@0", "Get this marker out")]
        [InlineData("Get this marker out@1", "Get this marker out")]
        [InlineData("Get this marker out@2", "Get this marker out")]
        [InlineData("Get this marker out@3", "Get this marker out")]
        [InlineData("a bit of the the everything in a a possible thing@3", "A bit of the everything in a possible thing")]
        [InlineData("","")]
        public void FitBlackCard_AnyCard_CorrectlyAdapted(string card, string expected)
        {
            // Arrange
            var fitter = new CombinedCardFitter(new Mock<ILogService>().Object);

            // Act
            var result = fitter.FitCombinedCard(card);

            // Assert
            result.Should().Be(expected);
        }
    }
}
