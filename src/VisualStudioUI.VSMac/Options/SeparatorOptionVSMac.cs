// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using AppKit;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class SeparatorOptionVSMac : OptionVSMac
    {
        private NSView? _separatorView;

        public SeparatorOptionVSMac(SeparatorOption option) : base(option)
        {
        }

        public SeparatorOption SeparatorOption => ((SeparatorOption)Option);

        public override NSView View
        {
            get
            {
                if (_separatorView == null)
                {
                    _separatorView = CreateView();
                }

                return _separatorView;
            }
        }

        private NSView CreateView()
        {
            // View:     separatorView
            var separatorView = new NSView();
            separatorView.WantsLayer = true;
            separatorView.TranslatesAutoresizingMaskIntoConstraints = false;

            var separatorViewWidthConstraint = separatorView.WidthAnchor.ConstraintEqualTo(600f);
            separatorViewWidthConstraint.Active = true;
            var separatorViewHeightConstraint = separatorView.HeightAnchor.ConstraintEqualTo(31f);
            separatorViewHeightConstraint.Active = true;

            // View:     boxView
            var boxView = new NSBox();
            boxView.BoxType = NSBoxType.NSBoxSeparator;
            boxView.Title = "";
            boxView.TranslatesAutoresizingMaskIntoConstraints = false;

            separatorView.AddSubview(boxView);
            var boxViewWidthConstraint = boxView.WidthAnchor.ConstraintEqualTo(600f);
            boxViewWidthConstraint.Priority = (int) NSLayoutPriority.DefaultLow;
            boxViewWidthConstraint.Active = true;
            var boxViewHeightConstraint = boxView.HeightAnchor.ConstraintEqualTo(1f);
            boxViewHeightConstraint.Active = true;

            boxView.TrailingAnchor.ConstraintEqualTo(separatorView.TrailingAnchor, 0f).Active = true;
            boxView.LeadingAnchor.ConstraintEqualTo(separatorView.LeadingAnchor, 0f).Active = true;
            boxView.TopAnchor.ConstraintEqualTo(separatorView.CenterYAnchor, -0.5f).Active = true;

            return separatorView;
        }
    }
}
