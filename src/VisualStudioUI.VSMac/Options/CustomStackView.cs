using AppKit;

/// <summary>
/// This file is copied & simplified from NSCustomStackView.cs in VSMac
/// </summary>

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class CustomStackView : AppKit.NSStackView
    {
        public override bool IsFlipped => true;

        public CustomStackView(AppKit.NSUserInterfaceLayoutOrientation orientation = NSUserInterfaceLayoutOrientation.Vertical, float spacing = 10)
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            Orientation = orientation;
            Spacing = spacing;
            Distribution = NSStackViewDistribution.Fill;
            if (orientation == NSUserInterfaceLayoutOrientation.Vertical)
                Alignment = NSLayoutAttribute.Leading;
            else
                Alignment = NSLayoutAttribute.CenterY;
        }
    }
}
