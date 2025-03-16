namespace MementoMori.Server.Extensions;

public static class DeckExtensions
{
    public static List<string> TagsToString(this Deck deck)
    {
        List<string> tagsAsStrings = new List<string>();
        if (deck.Tags == null)
            return [];
        deck.Tags.ForEach(tag => { tagsAsStrings.Add(tag.ToString()); });
        return tagsAsStrings;
    }
}