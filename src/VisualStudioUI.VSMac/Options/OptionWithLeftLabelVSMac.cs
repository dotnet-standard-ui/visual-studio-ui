// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public abstract class OptionWithLeftLabelVSMac : OptionVSMac
    {
        private NSView? _optionView;
        private NSView? _control;
        private NSButton? _hintButton;
        private NSTextField? _label;

        public OptionWithLeftLabelVSMac(Option option) : base(option)
        {
        }

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

        protected abstract NSView ControlView { get; }

        private void CreateView()
        {
            // View:     optionView
            _optionView = new NSView();
            _optionView.WidthAnchor.ConstraintEqualToConstant(600f - IndentValue()).Active = true;

            _control = ControlView;

            _optionView.AddSubview(_control);

            if (Option.ValidationMessage != null)
            {
                Option.ValidationMessage.PropertyChanged += delegate { UpdateHintButton(); };
            }
            UpdateHintButton();

            _label = CreateLabelView();
            if (_label != null)
            {
                _optionView.AddSubview(_label);

                _label.TrailingAnchor.ConstraintEqualToAnchor(_control.LeadingAnchor, -8f).Active = true;
                _label.CenterYAnchor.ConstraintEqualToAnchor(_control.CenterYAnchor).Active = true;
            }

            NSTextField? descriptionView = CreateDescriptionView();
            if (descriptionView != null)
            {
                _optionView.AddSubview(descriptionView);
                descriptionView.LeadingAnchor.ConstraintEqualToAnchor(_control.LeadingAnchor).Active = true;
                descriptionView.TopAnchor.ConstraintEqualToAnchor(_control.BottomAnchor, 5f).Active = true;
                descriptionView.BottomAnchor.ConstraintEqualToAnchor(_optionView.BottomAnchor,-5).Active = true;
            }
            else
            {
                _control.BottomAnchor.ConstraintEqualToAnchor(_optionView.BottomAnchor, -5f).Active = true;
            }

            float leftSpace = (Option.AllowSpaceForLabel||_label != null) ? 222f : 20f;
            _control.LeadingAnchor.ConstraintEqualToAnchor(_optionView.LeadingAnchor, leftSpace + IndentValue()).Active = true;
            _control.TopAnchor.ConstraintEqualToAnchor(_optionView.TopAnchor, 5f).Active = true;

        }

        public override void OnEnableChanged(bool enabled)
        {
            if (_label != null)
            {
                _label.TextColor = enabled ? NSColor.LabelColor : NSColor.DisabledControlText;
            }
        }

        protected override void UpdateHintButton()
        {
            if (_hintButton != null)
            {
                _hintButton.RemoveFromSuperview();
                _hintButton = null;
            }

            _hintButton = CreateHintButton();

            if (_hintButton == null)
                return;

            _optionView!.AddSubview(_hintButton);

            _hintButton.LeadingAnchor.ConstraintEqualToAnchor(_control!.TrailingAnchor, 10f).Active = true;
            _hintButton.CenterYAnchor.ConstraintEqualToAnchor(_control!.CenterYAnchor).Active = true;
        }
    }
}
