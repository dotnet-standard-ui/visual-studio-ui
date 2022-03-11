using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    /// <summary>
    /// A BarTextButton is for multiple buttons in a horizontal bar, like Add/Remove buttons
    /// below lists. It's copied from
    /// https://github.com/xamarin/vsmac/blob/main/main/src/core/MonoDevelop.Ide/MonoDevelop.Components/ButtonBarTextButton.cs
    /// </summary>
    class BarTextButton : NSButton
    {
        const int ContentMargin = 10;
        const int MinimumSize = 80;

        public override CGSize IntrinsicContentSize => new CGSize(Math.Max(MinimumSize, base.IntrinsicContentSize.Width + (ContentMargin * 2)), base.IntrinsicContentSize.Height);

        public BarTextButton()
        {
            BezelStyle = NSBezelStyle.RoundRect;
            TranslatesAutoresizingMaskIntoConstraints = false;
        }
    }
}