using CardsAgainstHyurmanity.Modules.WhiteCardFitting;
using DalamudBasics.Logging;
using FluentAssertions;
using Moq;

namespace Tests
{
    public class WhiteCardFittingTests : IClassFixture<CardFittingFixture>
    {
        public CardFittingFixture fixture { get; set; }

        public WhiteCardFittingTests(CardFittingFixture sharedFixture)
        {
            fixture = sharedFixture;
        }

        [Fact]
        public void FitWhiteCard_Present_CorrectChange()
        {
            // Arrange
            string blackCard = "Something _ something@0";

            // Act
            var result = fixture.fitter.AdaptWhiteCards(blackCard, fixture.SourceCards);

            // Assert
            result.Should().BeEquivalentTo(fixture.ExpectedPresent);
        }

        [Fact]
        public void FitWhiteCard_Past_CorrectChange()
        {
            // Arrange
            string blackCard = "Something _ something@1";

            // Act
            var result = fixture.fitter.AdaptWhiteCards(blackCard, fixture.SourceCards);

            // Assert
            result.Should().BeEquivalentTo(fixture.ExpectedPast);
        }

        [Fact]
        public void FitWhiteCard_PastPartifiple_CorrectChange()
        {
            // Arrange
            string blackCard = "Something _ something@2";

            // Act
            var result = fixture.fitter.AdaptWhiteCards(blackCard, fixture.SourceCards);

            // Assert
            result.Should().BeEquivalentTo(fixture.ExpectedPastParticiple);
        }

        [Fact]
        public void FitWhiteCard_Infinitive_CorrectChange()
        {
            // Arrange
            string blackCard = "Something _ something@3";

            // Act
            var result = fixture.fitter.AdaptWhiteCards(blackCard, fixture.SourceCards);

            // Assert
            result.Should().BeEquivalentTo(fixture.ExpectedInfinitive);
        }
    }
}
