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
        [InlineData("fake duplicate a, gamba and", "Fake duplicate a, gamba and")]
        [InlineData("fake and real double a, gamba and a a duplicate", "Fake and real double a, gamba and a duplicate")]
        [InlineData("fake the writhe a", "Fake the writhe a")]
        [InlineData("fake the writhe the", "Fake the writhe the")]
        [InlineData("fake a bola the thing", "Fake a bola the thing")]
        [InlineData("fake a bola a thing", "Fake a bola a thing")]
        [InlineData("fake and real the the writhe the", "Fake and real the writhe the")]
        [InlineData("damn the the allagans", "Damn the allagans")]
        [InlineData("damn the theory", "Damn the theory")]
        [InlineData("damn a theory", "Damn a theory")]
        [InlineData("try a an theory", "Try an theory")]
        [InlineData("try the an offer","Try the offer")]
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
