using System;
using Xunit;
using MementoMori.API.Entities;
using MementoMori.API.Services;

namespace MementoMori.Tests.UnitTests.ServiceTests
{
    public class SpacedRepetitionTests
    {
        private readonly SpacedRepetition _spacedRepetition;

        public SpacedRepetitionTests()
        {
            _spacedRepetition = new SpacedRepetition();
        }

        [Fact]
        public void UpdateCard_ShouldIncreaseRepetitions_WhenQualityIsHigh()
        {
            var card = new UserCardData
            {
                Repetitions = 1,
                Interval = 6,
                EaseFactor = 2.5,
                LastReviewed = DateTime.Now.AddDays(-7)
            };
            int quality = 6;

            var updatedCard = _spacedRepetition.UpdateCard(card, quality);

            Assert.Equal(2, updatedCard.Repetitions);
            Assert.Equal(6, updatedCard.Interval); 
            Assert.True(updatedCard.EaseFactor > 2.5); 
        }

        [Fact]
        public void UpdateCard_ShouldResetRepetitionsAndInterval_WhenQualityIsLow()
        {
            var card = new UserCardData
            {
                Repetitions = 3,
                Interval = 10,
                EaseFactor = 2.5,
                LastReviewed = DateTime.Now.AddDays(-11)
            };
            int quality = 2;

            var updatedCard = _spacedRepetition.UpdateCard(card, quality);

            Assert.Equal(0, updatedCard.Repetitions);
            Assert.Equal(1, updatedCard.Interval);
        }


        [Fact]
        public void IsDueForReview_ShouldReturnTrue_WhenCardIsDue()
        {
            var card = new UserCardData
            {
                LastReviewed = DateTime.Now.AddDays(-5),
                Interval = 3
            };

            var isDue = _spacedRepetition.IsDueForReview(card);

            Assert.True(isDue);
        }

        [Fact]
        public void IsDueForReview_ShouldReturnFalse_WhenCardIsNotDue()
        {
            var card = new UserCardData
            {
                LastReviewed = DateTime.Now.AddDays(-1),
                Interval = 3
            };

            var isDue = _spacedRepetition.IsDueForReview(card);

            Assert.False(isDue);
        }

        [Fact]
        public void UpdateCard_SetsIntervalToOne_WhenRepetitionsIsZeroAndQualityIsThreeOrMore()
        {
            var card = new UserCardData
            {
                Repetitions = 0,
                Interval = 0,
                EaseFactor = 2.5
            };
            int quality = 3;

            var updatedCard = _spacedRepetition.UpdateCard(card, quality);

            Assert.Equal(1, updatedCard.Interval);
            Assert.Equal(1, updatedCard.Repetitions);
        }

        [Fact]
        public void UpdateCard_RecalculatesInterval_WhenRepetitionsAreMoreThanOne()
        {
            var card = new UserCardData
            {
                Repetitions = 2,
                Interval = 6,
                EaseFactor = 2.5
            };
            int quality = 4;

            var expectedInterval = (int)Math.Round(6 * 2.5);

            var updatedCard = _spacedRepetition.UpdateCard(card, quality);

            Assert.Equal(expectedInterval, updatedCard.Interval);
            Assert.Equal(3, updatedCard.Repetitions);
        }

    }
}
