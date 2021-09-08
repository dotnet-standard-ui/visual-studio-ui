namespace Microsoft.VisualStudioUI.Options.Models
{
    public class KeyValueItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public KeyValueItem(string variable, string value, string type = "")
        {
            Key = variable;
            Value = value;
            Type = type;
        }
    }
}
