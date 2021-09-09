using System;
using System.Collections.Immutable;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.Options
{
    public class KeyValueTypeTableOption : Option
    {
        public string TypeColumnTitle = string.Empty;
        public string KeyColumnTitle = string.Empty;
        public string ValueColumnTitle = string.Empty;
        public string AddButtonTitle = string.Empty;
        public string RemoveButtonTitle = string.Empty;
        public string EditButtonTitle = string.Empty;
        public string AddToolTip = string.Empty;
        public string RemoveToolTip = string.Empty;
        public string EditToolTip = string.Empty;

        public ViewModelProperty<ImmutableArray<KeyValueItem>> Property { get; }
        public ViewModelProperty<KeyValueItem> SelectedProperty { get;}

        public event EventHandler? AddClicked;
        public event EventHandler? EditClicked;
        public event EventHandler? RemoveClicked;

        public void AddInvoke(object sender, EventArgs e)
        {
            AddClicked?.Invoke(sender, e);
        }

        public void EditInvoke(object sender, EventArgs e)
        {
            EditClicked?.Invoke(sender, e);
        }

        public void RemoveInvoke(object sender, EventArgs e)
        {
            RemoveClicked?.Invoke(sender, e);
        }

        public KeyValueTypeTableOption(ViewModelProperty<KeyValueItem> selectedProperty, ViewModelProperty<ImmutableArray<KeyValueItem>> property)
        {
            Property = property;
            Property.Bind();
            SelectedProperty = selectedProperty;
            SelectedProperty.Bind();
            Platform = OptionFactoryPlatform.Instance.CreateKeyValueTypeTableOptionPlatform(this);
        }
    }

}
