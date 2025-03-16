using MementoMori.API.Models;

namespace MementoMori.API;

public interface ISpacedRepetition
{
    UserCardData UpdateCard(UserCardData card, int quality);
    bool IsDueForReview(UserCardData card);    
}
