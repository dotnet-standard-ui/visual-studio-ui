using AppKit;

/// <summary>
/// This file is copied & simplified from NSCustomViews.cs in VSMac
/// </summary>

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    internal class MultilineButton : CustomStackView
    {
        readonly internal NSButton primaryControl;
        readonly internal NSLabel secondaryLabel;

        public bool Enabled
        {
            get => primaryControl.Enabled;
            set => primaryControl.Enabled = value;
        }

        public string Title
        {
            get => secondaryLabel.StringValue;
            set => secondaryLabel.StringValue = value;
        }

        public bool AllowsMixedState
        {
            get => primaryControl.AllowsMixedState;
            set => primaryControl.AllowsMixedState = value;
        }

        public string ControlAccessibilityHelp
        {
            get => primaryControl.AccessibilityHelp;
            set => primaryControl.AccessibilityHelp = value ?? string.Empty;
        }

        public MultilineButton() : base(NSUserInterfaceLayoutOrientation.Horizontal, 5)
        {
            Alignment = NSLayoutAttribute.Top;

            primaryControl = new AppKit.NSButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Title = string.Empty
            };
            this.AddArrangedSubview(primaryControl);

            primaryControl.SetContentHuggingPriorityForOrientation((int)NSLayoutPriority.DefaultHigh, NSLayoutConstraintOrientation.Horizontal);

            secondaryLabel = new NSLabel();
            this.AddArrangedSubview(secondaryLabel);
            secondaryLabel.Cell.AccessibilityElement = false;
            secondaryLabel.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            secondaryLabel.SetContentCompressionResistancePriority((int)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Horizontal);

            // Align the image centerY to the centerY of the first line of text
            var offset = secondaryLabel.Font.CapHeight / 2;
            primaryControl.CenterYAnchor.ConstraintEqualTo(secondaryLabel.FirstBaselineAnchor, -offset).Active = true;
        }

        internal void SetButtonType(NSButtonType type)
        {
            primaryControl.SetButtonType(type);
        }
    }

    public class NSLabel : NSTextField
	{
		public NSLabel (string value = null)
		{
			Editable = false;
			Bordered = false;
			Bezeled = false;
			DrawsBackground = false;
			Selectable = false;
			TranslatesAutoresizingMaskIntoConstraints = false;

			if (!string.IsNullOrEmpty(value))
            {
				StringValue = value;
            }
		}
	}
}
