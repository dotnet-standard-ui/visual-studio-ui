namespace Microsoft.VisualStudioUI.Options.Models
{
    public class KeyValueItem
    {
        public string Variable { get; set; }
        public string Value { get; set; }

        public KeyValueItem(string variable, string value)
        {
            Variable = variable;
            Value = value;
        }
    }
}
