namespace Microsoft.VisualStudioUI.Options.Models
{
    public class EnviroumentVariableItem
    {
        public string Variable { get; set; }
        public string Value { get; set; }

        public EnviroumentVariableItem(string variable, string value)
        {
            Variable = variable;
            Value = value;
        }
    }
}
