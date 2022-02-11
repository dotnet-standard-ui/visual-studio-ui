using AppKit;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class SwitchOptionVSMac : OptionVSMac
    {
        private NSView? _optionView;
        private NSSwitch? _switchButton;
        private HintPopover? _hintPopover;
        private NSProgressIndicator? _progressIndicator;
        private NSLayoutConstraint? _descriptionBottomConstraints;

        public SwitchOptionVSMac(SwitchOption option) : base(option)
        {
        }

        public SwitchOption SwitchOption => ((SwitchOption)Option);

        public override NSView View
        {
            get
            {
                if (_optionView == null)
                {
                    CreateView();
                }

                return _optionView!;
            }
        }

        private void CreateView()
        {
            bool enable = ((SwitchOption)Option).Property.Value;

            _optionView = new NSStackView() { Orientation = NSUserInterfaceLayoutOrientation.Vertical };
            _optionView.WantsLayer = true;
            _optionView.TranslatesAutoresizingMaskIntoConstraints = false;

            _switchButton = new NSSwitch();
            _switchButton.ControlSize = NSControlSize.Regular;
            _switchButton.State = enable ? 1 : 0;
            _switchButton.TranslatesAutoresizingMaskIntoConstraints = false;

            _switchButton.Activated += (s, e) => {
                ((SwitchOption)Option).Property.Value = (_switchButton.State == 1);
            };

            SwitchOption.Property.PropertyChanged += delegate {
                _switchButton.State = ((SwitchOption)Option).Property.Value ? 1 : 0;
            };

            SwitchOption.ShowSpinner.PropertyChanged += delegate {

                if (SwitchOption.ShowSpinner.Value)
                {
                    _progressIndicator!.StartAnimation(null);
                }
                else
                {
                    _progressIndicator!.StopAnimation(null);
                }
            };

            _optionView.AddSubview(_switchButton);
            _switchButton.WidthAnchor.ConstraintEqualTo(38f).Active = true;
            _switchButton.HeightAnchor.ConstraintEqualTo(21f).Active = true;
            _switchButton.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, 68f).Active = true;
            _switchButton.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor, 30f).Active = true;

            string? buttonLabel = SwitchOption.ButtonLabel;
            // Temporarily use Label as a fallback; remove when no longer needed
            if (string.IsNullOrEmpty(buttonLabel))
                buttonLabel = SwitchOption.Label;
            if (buttonLabel == null)
                buttonLabel = "";

            SetAccessibilityTitleToLabel(_switchButton, buttonLabel);

            var title = new NSTextField();
            title.Editable = false;
            title.Bordered = false;
            title.DrawsBackground = false;
            title.StringValue = buttonLabel!;
            title.Alignment = NSTextAlignment.Left;
            title.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize, NSFontWeight.Bold);
            title.TextColor = NSColor.LabelColor;
            title.TranslatesAutoresizingMaskIntoConstraints = false;
            title.SizeToFit();

            _optionView.AddSubview(title);

            title.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, 121f).Active = true;
            title.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor, 24f).Active = true;

            _progressIndicator = new NSProgressIndicator(new CoreGraphics.CGRect(0, 0, 18, 18));
            _progressIndicator.Style = NSProgressIndicatorStyle.Spinning;
            _progressIndicator.ControlSize = NSControlSize.Small;
            _progressIndicator.IsDisplayedWhenStopped = false;
            _progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
            _optionView.AddSubview(_progressIndicator);

            _progressIndicator.LeadingAnchor.ConstraintEqualTo(title.TrailingAnchor, 6).Active = true;
            _progressIndicator.TopAnchor.ConstraintEqualTo(title.TopAnchor).Active = true;

            string? descriptionText = Option.Description;
            if (descriptionText == null)
                descriptionText = Option.Name;

            NSTextField? description = null;
            if (!string.IsNullOrEmpty(descriptionText))
            {
                description = new NSTextField();
                description.Editable = false;
                description.Bordered = false;
                description.DrawsBackground = false;
                description.PreferredMaxLayoutWidth = 1;
                description.StringValue = descriptionText!;
                description.Alignment = NSTextAlignment.Left;
                description.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
                description.TextColor = NSColor.SecondaryLabelColor;
                description.TranslatesAutoresizingMaskIntoConstraints = false;
                description.LineBreakMode = NSLineBreakMode.ByWordWrapping;
                _optionView.AddSubview(description);

                description.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, 121f).Active = true;
                description.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor, 42f).Active = true;

                var bestHeight = description.Cell.CellSizeForBounds(new CoreGraphics.CGRect(0, 0, 354, 600)).Height;
                var _descriptionHeightConstraint = description.HeightAnchor.ConstraintEqualTo(bestHeight);
                _descriptionHeightConstraint.Active = true;
                var _descriptionWidthConstraint = description.WidthAnchor.ConstraintEqualTo(354f);
                _descriptionWidthConstraint.Active = true;

                float bottomeSpace = -28;
                if (bestHeight > 26) { bottomeSpace = -14; }
                _descriptionBottomConstraints = description.BottomAnchor.ConstraintEqualTo(_optionView.BottomAnchor, bottomeSpace);
                _descriptionBottomConstraints.Active = true;
            }

            if (!string.IsNullOrEmpty(Option.Hint))
            {
                var helpButton = new NSButton();
                helpButton.BezelStyle = NSBezelStyle.HelpButton;
                helpButton.Title = "";
                helpButton.ControlSize = NSControlSize.Regular;
                helpButton.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                helpButton.TranslatesAutoresizingMaskIntoConstraints = false;

                _optionView.AddSubview(helpButton);
                var _helpButtonWidthConstraint = helpButton.WidthAnchor.ConstraintEqualTo(21f);
                _helpButtonWidthConstraint.Priority = (int)NSLayoutPriority.DefaultLow;
                _helpButtonWidthConstraint.Active = true;

                helpButton.TrailingAnchor.ConstraintEqualTo(_optionView.TrailingAnchor, -6f).Active = true;
                helpButton.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor, 30f).Active = true;
                helpButton.Activated += (o, args) => ShowHintPopover(Option.Hint!, helpButton);
            }

            var _optionViewWidthConstraint = _optionView.WidthAnchor.ConstraintEqualTo(600f);
            _optionViewWidthConstraint.Active = true;

        }


        public override void OnEnableChanged(bool enabled)
        {
            _switchButton!.Enabled = enabled;
        }


        private void ShowHintPopover(string message, NSButton button)
        {
            _hintPopover?.Close();
            _hintPopover?.Dispose();
            _hintPopover = new HintPopover(message, null);
            _hintPopover.MaxWidth = 256;
            //TODO:
            //popover.SetAppearance (Appearance);

            var bounds = button.Bounds;
            _hintPopover.Show(bounds, button, NSRectEdge.MaxYEdge);
        }
    }
}
