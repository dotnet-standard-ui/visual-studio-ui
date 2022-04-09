using System.Collections.Immutable;

namespace Microsoft.VisualStudioUI.Options.Models
{
    public delegate string ItemDisplayStringFunc<in TItem>(TItem? item) where TItem : class;
    public delegate bool ItemIsBoldFunc<in TItem>(TItem? item) where TItem : class;

    public static class DisplayableItemsUtil
    {
        public static string ItemDisplayStringFromToString<TItem>(TItem? item) where TItem : class
        {
            if (item == null)
                return "";

            return item.ToString();
        }

        public static TItem? FindMatch<TItem>(ImmutableArray<TItem> items, string? displayString,
            ItemDisplayStringFunc<TItem> itemDisplayStringFunc) where TItem : class
        {
            if (displayString == null)
                return null;

            foreach (TItem item in items)
            {
                string itemDisplayString = itemDisplayStringFunc(item);
                if (itemDisplayString.Equals(displayString))
                    return item;
            }

            return null;
        }

        public static string? FindMatch(ImmutableArray<string> items, string? displayString)
        {
            if (displayString == null)
                return null;

            foreach (string item in items)
            {
                if (item.Equals(displayString))
                    return item;
            }

            return null;
        }
    }
}
