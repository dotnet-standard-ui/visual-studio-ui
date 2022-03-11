// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class OptionCardVSMac : OptionCardPlatform
    {
        private NSView? _cardView;

        public OptionCardVSMac(OptionCard optionCard) : base(optionCard)
        {
        }

        public NSView View
        {
            get
            {
                if (_cardView == null)
                    _cardView = CreateView();
                return _cardView;
            }
        }

        private NSView CreateView()
        {
            // View:     card
            var cardView = new NSView
            {
                WantsLayer = true,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // View:     background
            var background = new NSBox();
            background.BoxType = NSBoxType.NSBoxCustom;
            background.FillColor = NSColor.AlternatingContentBackgroundColors[1];
            background.BorderColor = NSColor.SeparatorColor;
            background.BorderWidth = 1;
            background.CornerRadius = 8;
            background.Title = "";
            background.TranslatesAutoresizingMaskIntoConstraints = false;

            cardView.AddSubview(background);

            background.TrailingAnchor.ConstraintEqualTo(cardView.TrailingAnchor, 0f).Active = true;
            background.LeadingAnchor.ConstraintEqualTo(cardView.LeadingAnchor, 0f).Active = true;
            background.BottomAnchor.ConstraintEqualTo(cardView.BottomAnchor, 0f).Active = true;
            background.TopAnchor.ConstraintEqualTo(cardView.TopAnchor, 0f).Active = true;

            // View:     titleLabel
            var titleOffset = 0;
            if (!string.IsNullOrEmpty(OptionCard.Label))
            {
                var titleLabel = new NSTextField();
                titleLabel.Editable = false;
                titleLabel.Bordered = false;
                titleLabel.DrawsBackground = false;
                titleLabel.PreferredMaxLayoutWidth = 1;
                titleLabel.StringValue = OptionCard.Label ?? "";
                titleLabel.Alignment = NSTextAlignment.Left;
                titleLabel.Font =
                    NSFont.SystemFontOfSize(NSFont.SystemFontSize, NSFontWeight.Bold);
                titleLabel.TextColor = NSColor.LabelColor;
                titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;

                cardView.AddSubview(titleLabel);
                var titleLabelWidthConstraint = titleLabel.WidthAnchor.ConstraintEqualTo(370f);
                titleLabelWidthConstraint.Active = true;
                var titleLabelHeightConstraint = titleLabel.HeightAnchor.ConstraintEqualTo(16f);
                titleLabelHeightConstraint.Active = true;

                titleLabel.LeadingAnchor.ConstraintEqualTo(cardView.LeadingAnchor, 24f).Active = true;
                titleLabel.TopAnchor.ConstraintEqualTo(cardView.TopAnchor, 17f).Active = true;

                //top position offset for card with a title
                titleOffset = 33;
            }

            // View:     optionsStackView
            var optionsStackView = new NSStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                EdgeInsets = new NSEdgeInsets(0, 0, 0, 0),
                Spacing = 0f,
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Distribution = NSStackViewDistribution.Fill
            };

            cardView.AddSubview(optionsStackView);
            optionsStackView.WidthAnchor.ConstraintEqualTo(600f).Active = true;

            optionsStackView.LeadingAnchor.ConstraintEqualTo(cardView.LeadingAnchor, 20f).Active = true;
            optionsStackView.BottomAnchor.ConstraintEqualTo(cardView.BottomAnchor, -20f).Active = true;
            optionsStackView.TopAnchor.ConstraintEqualTo(cardView.TopAnchor, 20f + titleOffset).Active = true;

            foreach (Option option in OptionCard.Options)
            {
                NSView optionView = ((OptionVSMac)option.Platform).View;

                // If the option's visibility is dynamic, hide if it's not initially showing and update the hidden
                // property the desired visibility changes. For NSStackViews, hidden views are automatically detached,
                // excluded from the layout.

                UpdateOptionVisible(option);

                ToggleButtonOption? visibilityDependsOn = option.VisibilityDependsOn;
                if (visibilityDependsOn != null)
                {
                    visibilityDependsOn.Property.PropertyChanged += delegate { UpdateOptionVisible(option); };
                }

                ViewModelProperty<bool>? visible = option.Visible;
                if (visible != null)
                {
                    visible.PropertyChanged += delegate { UpdateOptionVisible(option); };
                }

                ToggleButtonOption? disablebilityDependsOn = option.DisablebilityDependsOn;
                if (disablebilityDependsOn != null)
                {
                    ViewModelProperty<bool> buttonProperty = disablebilityDependsOn.Property;

                    if (!buttonProperty.Value)
                        option.Platform.OnEnableChanged(buttonProperty.Value);

                    buttonProperty.PropertyChanged += delegate
                    {
                        option.Platform.OnEnableChanged(buttonProperty.Value);
                    };
                }

                ViewModelProperty<bool>? enable = option.Enable;
                if (enable != null)
                {
                    if (!enable.Value)
                        option.Platform.OnEnableChanged(enable.Value);

                    enable.PropertyChanged += delegate
                    {
                        option.Platform.OnEnableChanged(enable.Value);
                    };
                }

                optionsStackView.AddArrangedSubview(optionView);
            }

            return cardView;
        }

        private void UpdateOptionVisible(Option option)
        {
            NSView optionView = ((OptionVSMac)option.Platform).View;

            ViewModelProperty<bool>? toggleButtonProperty = option.VisibilityDependsOn?.Property;
            ViewModelProperty<bool>? visibleProperty = option.Visible;

            bool visible =
                (toggleButtonProperty == null || toggleButtonProperty.Value) &&
                (visibleProperty == null || visibleProperty.Value);

            optionView.Hidden = !visible;
        }
    }
}
