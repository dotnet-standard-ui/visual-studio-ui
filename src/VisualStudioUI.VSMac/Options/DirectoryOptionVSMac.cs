// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class DirectoryOptionVSMac : OptionWithLeftLabelVSMac
    {
        private NSStackView _controlView;
        private NSTextField _textField;
        private NSButton _button;

        public DirectoryOptionVSMac(DirectoryOption option) : base(option)
        {
        }

        public DirectoryOption DirectoryOption => ((DirectoryOption)Option);

        protected override NSView ControlView
        {
            get
            {
                if (_controlView == null)
                {
                    _controlView = new NSStackView
                    {
                        Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };

                    ViewModelProperty<string> property = DirectoryOption.Property;

                    _textField = new NSTextField
                    {
                        Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize),
                        StringValue = property.Value ?? string.Empty,
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        Editable = true,
                        Bordered = true,
                        DrawsBackground = true,
                        LineBreakMode = NSLineBreakMode.TruncatingTail
                    };
                    SetAccessibilityTitleToLabel(_textField);

                    _controlView.AddArrangedSubview(_textField);

                    property.PropertyChanged += delegate (object o, ViewModelPropertyChangedEventArgs args)
                    {
                        _textField.StringValue = ((string)args.NewValue) ?? string.Empty;
                    };

                    _textField.Changed += delegate { property.Value = _textField.StringValue; };

                    _button = new NSButton
                    {
                        BezelStyle = NSBezelStyle.RoundRect,
                        Bordered = true,
                        LineBreakMode = NSLineBreakMode.TruncatingTail,
                        Title = "···"
                    };
                    _button.SizeToFit();

                    _button.Activated += (s, e) =>
                    {
                        var openPanel = new NSOpenPanel
                        {
                            CanChooseDirectories = true,
                            CanChooseFiles = true
                        };
                        var response = openPanel.RunModal();
                        if (response == 1 && openPanel.Url != null)
                        {
                            _textField.StringValue = openPanel.Filename;
                            property.Value = _textField.StringValue;
                        }
                    };

                    _controlView.AddArrangedSubview(_button);
                    _controlView.HeightAnchor.ConstraintEqualToConstant(21f).Active = true;

                    _textField.WidthAnchor.ConstraintEqualToConstant(196f).Active = true;
                    _textField.HeightAnchor.ConstraintEqualToConstant(21).Active = true;
                    _textField.LeadingAnchor.ConstraintEqualToAnchor(_controlView.LeadingAnchor).Active = true;
                    _textField.TopAnchor.ConstraintEqualToAnchor(_controlView.TopAnchor).Active = true;
                    _button.WidthAnchor.ConstraintEqualToConstant(24f).Active = true;
                    _button.HeightAnchor.ConstraintEqualToConstant(21).Active = true;
                    _button.TrailingAnchor.ConstraintEqualToAnchor(_controlView.TrailingAnchor).Active = true;
                }
                return _controlView;
            }
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);

            _textField.Enabled = enabled;

            _button.Enabled = enabled;
        }
    }
}
