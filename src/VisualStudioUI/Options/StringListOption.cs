using System;
using System.Collections.Immutable;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.Options
{
    public class StringListOption : Option
    {
        public float Space = 10.0f;
        public float Width = 354.0f;
        public float Height = 72.0f;

        public string PrefixValue = string.Empty;
        public string DefaultValue = string.Empty;
        public string AddToolTip = string.Empty;
        public string RemoveToolTip = string.Empty;
        public bool Editable { get; set; } = true;

        public event EventHandler? ListChanged;
        public Func<object, EventArgs, string>? AddClicked = null;

        public ViewModelProperty<ImmutableArray<string>> Model { get; }

        public void ListChangedInvoke(object sender, EventArgs e)
        {
            ListChanged?.Invoke(sender, e);
        }

        public string? AddInvoke(object sender, EventArgs e)
        {
            return AddClicked?.Invoke(sender, e);
        }

        public StringListOption(ViewModelProperty<ImmutableArray<string>> model, string addToolTip = "", string removeToolTip = "", string label = "", string defaultListValue = "")
        {
            Label = label;
            DefaultValue = defaultListValue;
            AddToolTip = addToolTip;
            RemoveToolTip = removeToolTip;
            Model = model;
            Model.Bind();
            Platform = OptionFactoryPlatform.Instance.CreateStringListOptionPlatform(this);
        }
    }
}
