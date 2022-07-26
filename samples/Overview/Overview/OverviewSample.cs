using Microsoft.StandardUI;
using Microsoft.StandardUI.Controls;
using Microsoft.StandardUI.Shapes;
using static Microsoft.StandardUI.StandardUIStatics;

namespace Overview
{
    [StandardControl]
    public interface IOverviewSample : IStandardControl
    {
        Color SampleProperty { get; set; }
    }

    public class OverviewSampleImplementation : StandardControlImplementation<IOverviewSample>
    {
        public OverviewSampleImplementation(IOverviewSample control) : base(control)
        {
        }

        public override IUIElement Build() =>
            VerticalStack().Children(
                TextBlock()
                    .Text("Sample")
                    .FontSize(30)
                    .Foreground(SolidColorBrush(Colors.Purple)),

                TextBlock()
                    .Text("Learn about the .NET platform, create your first application, and deploy it to your device")
                    .FontSize(14)
                    .Foreground(SolidColorBrush(Colors.Gray)),

                Grid()
                    .ColumnDefinitions(
                        ColumnDefinition(),
                        ColumnDefinition(),
                        ColumnDefinition()
                    )
                    .RowDefinitions(
                        RowDefinition(),
                        RowDefinition(),
                        RowDefinition()
                    )._(
                        Rectangle().Width(50).Height(50).Fill(Colors.Red).GridCell(0, 0),
                        Rectangle().Width(50).Height(50).Fill(Colors.Green).GridCell(0, 1),
                        Rectangle().Width(50).Height(50).Fill(Colors.Yellow).GridCell(0, 2)
                    )
            );
    }
}
