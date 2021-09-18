using AppKit;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class ButtonOptionVSMac : OptionWithLeftLabelVSMac
    {
        private NSButton _button;

        public ButtonOptionVSMac(ButtonOption option) : base(option)
        {
        }

        public ButtonOption ButtonOption => ((ButtonOption)Option);


        protected override NSView ControlView
        {
            get
            {
                if (_button == null)
                {
                    _button = new NSButton();
                    _button.BezelStyle = NSBezelStyle.RoundRect;
                    _button.ControlSize = NSControlSize.Regular;
                    _button.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                    _button.Title = ButtonOption.ButtonLabel;
                    _button.TranslatesAutoresizingMaskIntoConstraints = false;
                    _button.SizeToFit();

                    if (!string.IsNullOrWhiteSpace(ButtonOption.PopoverMessage))
                        _button.Activated += (o, args) => ShowHintPopover(ButtonOption.PopoverMessage, _button, 500);
                    else
                        _button.Activated += ButtonOption.ButtonClicked;
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
    }
}
