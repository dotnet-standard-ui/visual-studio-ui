// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Foundation;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class StepperOptionVSMac : OptionWithLeftLabelVSMac
    {
        private NSView? _controlView;
        private NSTextField? _textField;
        private NSStepper? _stepper;
        private NSTextField? _leftLabel;


        public StepperOptionVSMac(StepperOption option) : base(option)
        {
        }

        public StepperOption StepperOption => ((StepperOption)Option);


        protected override NSView ControlView
        {
            get
            {
                if (_controlView == null)
                {
                    _controlView = new NSView()
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };

                    _leftLabel = CreateLabelView();

                    _stepper = new NSStepper()
                    {
                        IntValue = StepperOption.Property.Value,
                        MaxValue = StepperOption.Maximum,
                        MinValue = StepperOption.Minimum,
                        Increment = StepperOption.Increment,
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };

                    _stepper.Activated += (s, e) =>
                    {
                        _textField!.StringValue = _stepper.IntValue.ToString();
                        StepperOption.Property.Value = _stepper.IntValue;
                    };

                    _textField = new NSTextField()
                    {
                       
                        StringValue = StepperOption.Property.Value.ToString(),
                        Alignment = NSTextAlignment.Right,
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };
                    var format = new NSNumberFormatter
                    {
                        NumberStyle = NSNumberFormatterStyle.None
                    };
                    _textField.Formatter = format;

                    _textField.Changed += (s, e) =>
                    {
                        if (_textField.IntValue > StepperOption.Maximum)
                        {
                            _textField.IntValue = StepperOption.Maximum;
                        }
                        else if (_textField.IntValue < StepperOption.Minimum)
                        {
                            _textField.IntValue = StepperOption.Minimum;
                        }

                        StepperOption.Property.Value = _textField.IntValue;
                        _stepper.IntValue = _textField.IntValue;
                    };

                    StepperOption.Property.PropertyChanged += delegate {
                        _textField.IntValue = StepperOption.Property.Value;
                        _stepper.IntValue = StepperOption.Property.Value;
                    };

                    _controlView.AddSubview(_textField);
                    _controlView.AddSubview(_stepper);

                    _textField.WidthAnchor.ConstraintEqualTo(64f).Active = true;
                    _textField.HeightAnchor.ConstraintEqualTo(21f).Active = true;
                    _textField.LeadingAnchor.ConstraintEqualTo(_controlView.LeadingAnchor).Active = true;
                    _textField.TopAnchor.ConstraintEqualTo(_controlView.TopAnchor).Active = true;
                    _textField.BottomAnchor.ConstraintEqualTo(_controlView.BottomAnchor).Active = true;
                    _stepper.CenterYAnchor.ConstraintEqualTo(_textField.CenterYAnchor).Active = true;
                    _stepper.LeadingAnchor.ConstraintEqualTo(_textField.TrailingAnchor, 10).Active = true;
                    _controlView.TrailingAnchor.ConstraintEqualTo(_stepper.TrailingAnchor,2).Active = true;
                }
                return _controlView;
            }
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);

            if (_leftLabel != null)
                _leftLabel.Enabled = enabled;

            if (_textField != null)
                _textField.Enabled = enabled;

            _stepper!.Enabled = enabled;
        }

    }
}
