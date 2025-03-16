using MementoMori.API.Entities;

namespace MementoMori.API.Services;

public interface ISpacedRepetition
{
    UserCardData UpdateCard(UserCardData card, int quality);
    bool IsDueForReview(UserCardData card);    
}
