using System.Collections.Generic;
using System.IO;
using MementoMori.Server;
using MementoMori.Server.Models;

namespace MementoMori.Server.Service;

public class SpacedRepetition : ISpacedRepetition
{
        public UserCardData UpdateCard(UserCardData card, int quality)
        {

                quality = Math.Max(0, Math.Min(quality, 5));

                if (quality >= 3) // remembered
                {
                        if (card.Repetitions == 0)
                        {
                                card.Interval = 1;
                        }
                        else if (card.Repetitions == 1)
                        {
                                card.Interval = 6;
                        }
                        else
                        {
                                card.Interval = (int)Math.Round(card.Interval * card.EaseFactor);
                        }

                        card.EaseFactor += 0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02);
                        card.Repetitions++;
                }
                else // forgot
                {
                        card.Repetitions = 0;
                        card.Interval = 1;
                }

                card.LastReviewed = DateTime.Now;
                return card;
        }

        public bool IsDueForReview(UserCardData card)
        {
                return DateTime.Now >= card.LastReviewed.AddDays(card.Interval);
        }
}