using System.Collections.Immutable;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.Options
{
    /// <summary>
    /// A TextOption shows an editable string. The UI is typically an edit box.
    /// </summary>
    public class TextOption : Option
    {
        public string? PlaceholderText { get; }
        public bool AllowOnlyNumbers { get; set; } = false;

        public ImmutableArray<MacroMenuItem> MacroMenuItems { get; set; }

        public TextOption(ViewModelProperty<string> property, string? placeholder = null)
        {
            Property = property;
            PlaceholderText = placeholder;
            Platform = OptionFactoryPlatform.Instance.CreateTextOptionPlatform(this);
        }

        public ViewModelProperty<string> Property { get; }
    }
}
