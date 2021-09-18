// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace Microsoft.VisualStudioUI.Options
{
    /// <summary>
    /// This class will go away, being replaced by RadioButtonOption, CheckBoxOption, and SwitchOption. 
    /// </summary>
    public class ButtonOption : Option
    {
        public string ButtonLabel { get; set; } = "";
        public string PopoverMessage { get; set; } = "";

        public event EventHandler? Clicked;

        public void ButtonClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(sender, e);
        }

        public ButtonOption()
        {
            Platform = OptionFactoryPlatform.Instance.CreateButtonOptionPlatform(this);
        }
    }
}
