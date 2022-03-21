// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class CheckBoxOptionVSMac : OptionWithLeftLabelVSMac
    {
        private MultilineButton? _button;

        public CheckBoxOptionVSMac(CheckBoxOption option) : base(option)
        {
        }

        public CheckBoxOption CheckBoxOption => ((CheckBoxOption)Option);

        protected override NSView ControlView
        {
            get
            {
                if (_button == null)
                {
                    ViewModelProperty<bool> property = CheckBoxOption.Property;

                    _button = new MultilineButton();
                    _button.SetButtonType(NSButtonType.Switch);
                    _button.primaryControl.Activated += (o, args) => CheckBoxSelected();

                    _button.primaryControl.ControlSize = NSControlSize.Regular;
                    _button.Title = CheckBoxOption.ButtonLabel;
                    _button.primaryControl.AccessibilityTitle = CheckBoxOption.ButtonLabel;
                    _button.primaryControl.State = CheckBoxOption.Property.Value ? NSCellStateValue.On : NSCellStateValue.Off;
                    _button.secondaryLabel.WidthAnchor.ConstraintLessThanOrEqualTo(500f).Active = true;

                    property.PropertyChanged += delegate
                    {
                        _button.primaryControl.State = CheckBoxOption.Property.Value ? NSCellStateValue.On : NSCellStateValue.Off;
                    };
                }

                return _button!;
            }
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);

            if (_button != null)
                _button.Enabled = enabled;
        }

        private void CheckBoxSelected()
        {
            CheckBoxOption.Property.Value = (_button?.primaryControl.State == NSCellStateValue.On) ? true : false;
        }
    }
}
