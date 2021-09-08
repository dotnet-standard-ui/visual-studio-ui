using System.Collections.Immutable;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.Options
{
    public class KeyValueTableEntryOption: Option
    {
        public string KeyColumnTitle = string.Empty;
        public string ValueColumnTitle = string.Empty;
        public string AddButtonTitle = string.Empty;
        public string RemoveButtonTitle = string.Empty;
        public string AddToolTip = string.Empty;
        public string RemoveToolTip = string.Empty;

        public ViewModelProperty<ImmutableArray<KeyValueItem>> Property { get; }

        public KeyValueTableEntryOption(ViewModelProperty<ImmutableArray<KeyValueItem>> model)
        {
            Property = model;
            Property.Bind();
            Platform = OptionFactoryPlatform.Instance.CreateKeyValueTableEntryOptionPlatform(this);
        }
    }

}
