// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Foundation;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class TextOptionVSMac : OptionWithLeftLabelVSMac
    {
        private NSView? _controlView;
        private CustomTextField? _textField;
        private NSButton? _menuBtn;

        public TextOptionVSMac(TextOption option) : base(option)
        {
        }

        public TextOption TextOption => ((TextOption)Option);

        protected override NSView ControlView
        {
            get
            {
                if (_controlView == null)
                {
                    _controlView = new NSView
                    {
                        WantsLayer = true,
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };

                    ViewModelProperty<string> property = TextOption.Property;

                    _textField = new CustomTextField
                    {
                        Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize),
                        StringValue = property.Value ?? string.Empty,
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        PlaceholderString = TextOption.PlaceholderText ?? string.Empty,
                        AllowOnlyNumbers = TextOption.AllowOnlyNumbers,
                    };
                    _textField.LineBreakMode = NSLineBreakMode.TruncatingTail;
                    SetAccessibilityTitleToLabel(_textField);

                    _controlView.AddSubview(_textField);

                    property.PropertyChanged += delegate
                    {
                        _textField.StringValue = TextOption.Property.Value;
                    };

                    _textField.Changed += delegate { property.Value = _textField.StringValue; };

                    if (TextOption.MacroMenuItems != null)
                    {
                        _menuBtn = new NSButton() {
                            BezelStyle = NSBezelStyle.RoundRect,
                            Image = NSImage.ImageNamed("NSGoRightTemplate"),
                            TranslatesAutoresizingMaskIntoConstraints = false
                        };

                        _menuBtn.Activated += (sender, e) =>
                        {
                            NSEvent events = NSApplication.SharedApplication.CurrentEvent;
                            NSMenu.PopUpContextMenu(CreateMenu(), events, events.Window.ContentView);
                        };
                        _controlView.AddSubview(_menuBtn);

                        _menuBtn.WidthAnchor.ConstraintEqualToConstant(24f).Active = true;
                        _menuBtn.HeightAnchor.ConstraintEqualToConstant(21f).Active = true;
                        _menuBtn.TrailingAnchor.ConstraintEqualToAnchor(_controlView.TrailingAnchor).Active = true;
                        _menuBtn.CenterYAnchor.ConstraintEqualToAnchor(_controlView.CenterYAnchor).Active = true;
                        _controlView.WidthAnchor.ConstraintEqualToConstant(228f).Active = true;
                    }
                    else
                    {
                        _controlView.WidthAnchor.ConstraintEqualToConstant(196f).Active = true;
                    }

                    _controlView.HeightAnchor.ConstraintEqualToConstant(21).Active = true;
                    _textField.WidthAnchor.ConstraintEqualToConstant(196f).Active = true;
                    _textField.HeightAnchor.ConstraintEqualToConstant(21).Active = true;
                    _textField.LeadingAnchor.ConstraintEqualToAnchor(_controlView.LeadingAnchor).Active = true;
                    _textField.TopAnchor.ConstraintEqualToAnchor(_controlView.TopAnchor).Active = true;
                }

                return _controlView;
            }
        }

        private NSMenu CreateMenu()
        {
            NSMenu groupMenu = new NSMenu
            {
                AutoEnablesItems = false
            };

            foreach (var item in TextOption.MacroMenuItems)
            {
                if (string.IsNullOrWhiteSpace(item.Label)) continue;

                if (!"-".Equals(item.Label))
                {
                    NSMenuItem menuItem = new NSMenuItem
                    {
                        Title = item.Label
                    };
                    menuItem.Activated += (sender, e) =>
                    {
                        if (_textField != null)
                        {
                            _textField.StringValue = item?.MacroName + _textField.StringValue; // New value insert to head
                            TextOption.Property.Value = _textField.StringValue;
                        }
                    };

                    groupMenu.AddItem(menuItem);
                }
                else
                {
                    groupMenu.AddItem(NSMenuItem.SeparatorItem);
                }
            }

            return groupMenu;
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);
            if (_textField != null)
                _textField.Enabled = enabled;

            if (_menuBtn!= null)
                _menuBtn.Enabled = enabled;
        }
    }

    internal class CustomTextField : NSTextField
    {
        public bool AllowOnlyNumbers { get; set; } = false;

        public override void DidChange(NSNotification notification)
        {
            if (AllowOnlyNumbers)
            {
                StringValue = StringValue.Trim();
                if (StringValue.Length > 0)
                {
                    var newInput = StringValue[StringValue.Length - 1];
                    if (!int.TryParse(newInput.ToString(), out int n))
                    {
                        StringValue = StringValue.TrimEnd(newInput);
                    }
                }
            }

            base.DidChange(notification);
        }
    }
}
