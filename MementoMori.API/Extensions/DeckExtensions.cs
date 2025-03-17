using MementoMori.API.Entities;

namespace MementoMori.API.Extensions;

public static class DeckExtensions
{
    public static List<string> TagsToString(this Deck deck)
    {
        var tagsAsStrings = new List<string>();

        if (deck.Tags == null)
            return [];
        
        deck.Tags.ForEach(tag => { tagsAsStrings.Add(tag.ToString()); });
        
        return tagsAsStrings;
    }
}