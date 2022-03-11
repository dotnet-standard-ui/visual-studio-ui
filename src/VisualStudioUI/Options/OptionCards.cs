using System.Collections.Generic;

namespace Microsoft.VisualStudioUI.Options
{
    public class OptionCards
    {
        private readonly List<OptionCard> _cards = new List<OptionCard>();

        public OptionCardsPlatform Platform { get; }

        /// <summary>
        /// Indicates whether the cards should expand width wise to fill available space
        /// (that's the desired UI when in a dialog) or if they should
        /// have a fixed width (that's the desired UI when in a document window,
        /// where the width is normally much wider and taking it all can look odd).
        /// Even if UseFixedWidth is off, cards still have a minimum width.
        /// </summary>
        public bool UseFixedWidth { get; }

        public OptionCards(bool useFixedWidth = false)
        {
            Platform = OptionFactoryPlatform.Instance.CreateOptionCardsPlatform(this);
            UseFixedWidth = useFixedWidth;
        }

        public IReadOnlyList<OptionCard> Cards => _cards;

        public void AddCard(OptionCard card)
        {
            _cards.Add(card);
        }

        public void RemoveAllCards()
        {
            _cards.Clear();
        }
    }
}
