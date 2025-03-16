using MementoMori.Server.Models;

namespace MementoMori.Server;

public interface ISpacedRepetition
{
    UserCardData UpdateCard(UserCardData card, int quality);
    bool IsDueForReview(UserCardData card);    
}
