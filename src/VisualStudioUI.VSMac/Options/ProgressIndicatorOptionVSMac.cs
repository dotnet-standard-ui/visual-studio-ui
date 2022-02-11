// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class ProgressIndicatorOptionVSMac : OptionVSMac
    {
        private NSView _optionView;
        private NSProgressIndicator _progressIndicator;
        private NSStackView _childrenControl;

        public ProgressIndicatorOptionVSMac(ProgressIndicatorOption option) : base(option)
        {
        }

        public ProgressIndicatorOption ProgressIndicatorOption => ((ProgressIndicatorOption)Option);

        public override NSView View
        {
            get
            {
                if (_optionView == null)
                {
                    CreateView();
                }

                return _optionView;
            }
        }

        private void CreateView()
        {
            _optionView = new NSView();
            _optionView.WantsLayer = true;
            _optionView.TranslatesAutoresizingMaskIntoConstraints = false;

            _optionView.WidthAnchor.ConstraintEqualTo(600f).Active = true;
            _optionView.HeightAnchor.ConstraintEqualTo(100f).Active = true;

            if (!string.IsNullOrEmpty(Option.Label))
            {
                // View:     label
                var label = new NSTextField
                {
                    Editable = false,
                    Bordered = false,
                    DrawsBackground = false,
                    PreferredMaxLayoutWidth = 1,
                    // TODO: Make the colon be localization friendly
                    StringValue = Option.Label + ":",

                    Alignment = NSTextAlignment.Right,
                    Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize),
                    TextColor = NSColor.LabelColor,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };

                _optionView.AddSubview(label);
                label.WidthAnchor.ConstraintEqualTo(205f).Active = true;
                label.HeightAnchor.ConstraintEqualTo(16f).Active = true;

                label.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, 6f).Active = true;
                label.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor, 7f).Active = true;
            }

            _progressIndicator = new NSProgressIndicator(new CoreGraphics.CGRect(0, 0, 18, 18));
            _progressIndicator.Style = NSProgressIndicatorStyle.Spinning;
            _progressIndicator.ControlSize = NSControlSize.Small;
            _progressIndicator.IsDisplayedWhenStopped = false;
            _progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
            SetAccessibilityTitleToLabel(_progressIndicator);
            _progressIndicator.SizeToFit();
            _optionView.AddSubview(_progressIndicator);

            _progressIndicator.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, 215f).Active = true;
            _progressIndicator.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor, 5f).Active = true;

            // Default is start
            _progressIndicator.StartAnimation(null);

            ProgressIndicatorOption.ShowSpinner.PropertyChanged += SpinnerChanged;

            // Add other option element
            _childrenControl = new NSStackView()
            {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Distribution = NSStackViewDistribution.Fill,
                TranslatesAutoresizingMaskIntoConstraints = false,
            };

            if (ProgressIndicatorOption.Element != null)
            {
                NSView optionView = ((OptionVSMac)ProgressIndicatorOption.Element.Platform).View;
                _childrenControl.AddArrangedSubview(optionView);
            }

            _optionView.AddSubview(_childrenControl);

            _childrenControl.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor).Active = true;
            _childrenControl.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, _progressIndicator.FittingSize.Width).Active = true;
        }

        public void SpinnerChanged(object sender, ViewModelPropertyChangedEventArgs e)
        {
            if (ProgressIndicatorOption.ShowSpinner.Value)
            {
                _progressIndicator.StartAnimation(null);
            }
            else
            {
                _progressIndicator.StopAnimation(null);
            }
        }
    }
}
