// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class OptionCardsVSMac : OptionCardsPlatform
    {
        private NSStackView? _cardsStack;

        public OptionCardsVSMac(OptionCards optionCards) : base(optionCards)
        {
        }

        public NSView View
        {
            get
            {
                if (_cardsStack == null)
                    _cardsStack = CreateView();
                return _cardsStack;
            }
        }

        private NSStackView CreateView()
        {
            var cardsStack = new NSStackView();
            cardsStack.TranslatesAutoresizingMaskIntoConstraints = false;
            cardsStack.EdgeInsets = new NSEdgeInsets(0, 0, 0, 0);
            cardsStack.Alignment = NSLayoutAttribute.Leading;
            cardsStack.Spacing = 10f;
            cardsStack.Orientation = NSUserInterfaceLayoutOrientation.Vertical;
            cardsStack.Distribution = NSStackViewDistribution.Fill;

            NSLayoutConstraint widthConstraint;
            if (OptionCards.UseFixedWidth)
                widthConstraint = cardsStack.WidthAnchor.ConstraintEqualTo(680f);
            else widthConstraint = cardsStack.WidthAnchor.ConstraintGreaterThanOrEqualTo(680f);
            widthConstraint.Active = true;

            foreach (OptionCard card in OptionCards.Cards)
            {
                NSView cardView = ((OptionCardVSMac) card.Platform).View;
                cardsStack.AddArrangedSubview(cardView);
            }

            // The dummy view at the end grows/shrinks for the extra space
            var dummyView = new NSView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            cardsStack.AddArrangedSubview(dummyView);

            return cardsStack;
        }
    }
}