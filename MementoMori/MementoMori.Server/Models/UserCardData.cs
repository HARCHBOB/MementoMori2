using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MementoMori.Server.Models;

[PrimaryKey(nameof(CardId), nameof(DeckId), nameof(UserId))]
public class UserCardData
{
    public Guid CardId { get; set; }
    public Guid DeckId { get; set; }
    public Guid UserId { get; set; }

    public int Interval { get; set; } = 1;    

    public int Repetitions { get; set; } = 0; 

    public double EaseFactor { get; set; } = 2.5; 

    public DateTime LastReviewed { get; set; }

}