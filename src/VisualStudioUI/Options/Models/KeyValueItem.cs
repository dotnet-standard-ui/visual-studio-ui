namespace Microsoft.VisualStudioUI.Options.Models
{
    public class KeyValueItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public KeyValueItem(string key, string value, string type = "")
        {
            Key = key;
            Value = value;
            Type = type;
        }
    }
}
