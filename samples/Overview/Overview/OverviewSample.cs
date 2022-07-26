using Microsoft.StandardUI;
using Microsoft.StandardUI.Controls;
using Microsoft.StandardUI.Shapes;
using Microsoft.VisualStudioUI;
using static Microsoft.StandardUI.StandardUIStatics;
using static Microsoft.VisualStudioUI.VisualStudioUIStatics;

namespace Overview
{
    [StandardControl]
    public interface IOverviewSample : IStandardControl
    {
        Color SampleProperty { get; set; }
    }

    public class OverviewSampleImplementation : StandardControlImplementation<IOverviewSample>
    {
        private readonly int _blockWidth = 230;
        private readonly int _mainBlockHeight = 165;
        private readonly int _subBlockHeight = 26;
        private readonly int _cornerRadius = 3;

        public OverviewSampleImplementation(IOverviewSample control) : base(control)
        {
        }

        public override IUIElement Build() =>
            VerticalStack().Children(

                TextBlock()
                    .Text("Sample Title")
                    .FontSize(24)
                    .Foreground(SolidColorBrush(Color.FromHex("ffffff"))),

                VerticalStack().Margin(new Thickness(0, 12, 0, 0)).Height(50).Spacing(6).Children(
                    TextBlock()
                        .Text("Sample Line 1")
                        .Foreground(SolidColorBrush(Color.FromHex("ffffff")))
                        .FontSize(16),
                    TextBlock()
                        .Text("Sample Line 2")
                        .Foreground(SolidColorBrush(Color.FromHex("ffffff")))
                        .FontSize(16)
                    ),

                HorizontalStack().HorizontalAlignment(HorizontalAlignment.Left).Height(600).Margin(new Thickness(0, 32, 0, 0)).Children(
                        VerticalStack().Margin(new Thickness(0, 0, 24, 0)).Children(
                            Grid()._(
                                    Rectangle().Width(_blockWidth).Height(_mainBlockHeight + _subBlockHeight * 4.5).Fill(Color.FromHex("313131")).RadiusX(_cornerRadius).RadiusY(_cornerRadius),
                                    VerticalStack().Margin(new Thickness(0, 36, 0, 0)).Spacing(24).VerticalAlignment(VerticalAlignment.Top).HorizontalAlignment(HorizontalAlignment.Center).Children(
                                        Image().Source("pack://application:,,,/Overview;component/sample_icon.png").Width(48).Height(48),
                                        TextBlock().FontSize(12).HorizontalAlignment(HorizontalAlignment.Center).Text("Section 1").Foreground(SolidColorBrush(Color.FromHex("ffffff"))),
                                        VerticalStack().Margin(new Thickness(0, 4, 0, 0)).Children(

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 1")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 2")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 3")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 4")
                                                )
                                        )
                                    )
                                )
                        ),

                        VerticalStack().Margin(new Thickness(0, 0, 24, 0)).Children(
                            Grid()._(
                                    Rectangle().Width(_blockWidth).Height(_mainBlockHeight + _subBlockHeight * 3.5).Fill(Color.FromHex("313131")).RadiusX(_cornerRadius).RadiusY(_cornerRadius),
                                    VerticalStack().Margin(new Thickness(0, 36, 0, 0)).Spacing(24).VerticalAlignment(VerticalAlignment.Top).HorizontalAlignment(HorizontalAlignment.Center).Children(
                                        Image().Source("pack://application:,,,/Overview;component/sample_icon.png").Width(48).Height(48),
                                        TextBlock().FontSize(12).HorizontalAlignment(HorizontalAlignment.Center).Text("Section 2").Foreground(SolidColorBrush(Color.FromHex("ffffff"))),
                                        VerticalStack().Margin(new Thickness(0, 4, 0, 0)).Children(

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 1")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 2")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 3")
                                                )
                                        )
                                    )
                                )
                        ),

                        VerticalStack().Margin(new Thickness(0, 0, 0, 0)).Children(
                            Grid()._(
                                    Rectangle().Width(_blockWidth).Height(_mainBlockHeight + _subBlockHeight * 3.5).Fill(Color.FromHex("313131")).RadiusX(_cornerRadius).RadiusY(_cornerRadius),
                                    VerticalStack().Margin(new Thickness(0, 36, 0, 0)).Spacing(24).VerticalAlignment(VerticalAlignment.Top).HorizontalAlignment(HorizontalAlignment.Center).Children(
                                        Image().Source("pack://application:,,,/Overview;component/sample_icon.png").Width(48).Height(48),
                                        TextBlock().FontSize(12).HorizontalAlignment(HorizontalAlignment.Center).Text("Section 3").Foreground(SolidColorBrush(Color.FromHex("ffffff"))),
                                        VerticalStack().Margin(new Thickness(0, 4, 0, 0)).Children(

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 1")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 2")
                                                ),

                                            Grid().Height(_subBlockHeight)._(
                                                    HyperlinkButton().VerticalAlignment(VerticalAlignment.Center).HorizontalAlignment(HorizontalAlignment.Center).Uri("https://github.com/dotnet-standard-ui/visual-studio-ui").Label("Link 3")
                                                )
                                        )
                                    )
                                )
                        )
                    )
            );
    }
}
