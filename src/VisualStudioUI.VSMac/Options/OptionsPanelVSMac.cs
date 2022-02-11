using AppKit;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class OptionsPanelVSMac
    {
        private readonly NSScrollView _scrollView;

        public OptionsPanelVSMac(OptionCards optionCards)
        {
            _scrollView = new NSScrollView();
            _scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

            _scrollView.DrawsBackground = false;
            _scrollView.HasVerticalScroller = true;
            _scrollView.HasHorizontalScroller = false;
            _scrollView.AutohidesScrollers = true;

            var clipView = new FlippedClipView();
            clipView.DrawsBackground = false;
            _scrollView.ContentView = clipView;
            clipView.TranslatesAutoresizingMaskIntoConstraints = false;

            clipView.TopAnchor.ConstraintEqualTo(_scrollView.TopAnchor).Active = true;
            clipView.LeadingAnchor.ConstraintEqualTo(_scrollView.LeadingAnchor).Active = true;
            clipView.TrailingAnchor.ConstraintEqualTo(_scrollView.TrailingAnchor).Active = true;
            clipView.BottomAnchor.ConstraintEqualTo(_scrollView.BottomAnchor).Active = true;

            var documentView = new NSStackView();
            documentView.TranslatesAutoresizingMaskIntoConstraints = false;
            _scrollView.DocumentView = documentView;

            documentView.TopAnchor.ConstraintEqualTo(clipView.TopAnchor).Active = true;
            documentView.LeadingAnchor.ConstraintEqualTo(clipView.LeadingAnchor).Active = true;

            documentView.EdgeInsets = new NSEdgeInsets(24f, 24f, 24f, 24f);

            NSView optionsView = ((OptionCardsVSMac)optionCards.Platform).View;
            documentView.AddArrangedSubview(optionsView);
        }

        public NSView View => _scrollView;

        /*
        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            _documentView.Frame = new CGRect(0, 0, newSize.Width, newSize.Height);
        }
        */

        class FlippedClipView : NSClipView
        {
            public override bool IsFlipped => true;
        }

    }
}
