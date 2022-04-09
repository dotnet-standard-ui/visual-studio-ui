//
// OptionsComboBoxWidget.cs
//
// Author:
//       vstester <v-tianlz@microsoft.com>
//
// Copyright (c) 2021 ${CopyrightHolder}
//

using Microsoft.VisualStudioUI.Options.Models;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudioUI.Options
{
    /// <summary>
    /// A ComboBoxOption shows a list of items, where the user must choose one.
    /// Not selecting anything is also supported, indicated by Property.Value being
    /// null.
    /// </summary>
    /// <typeparam name="TItem">item type; items can be any type, with ItemDisplayStringFunc
    /// used to get the display string
    /// </typeparam>
    public class ComboBoxOption<TItem> : Option where TItem : class
    {
        public ViewModelProperty<TItem?> Property { get; }
        public ViewModelProperty<ImmutableArray<TItem>> ItemsProperty { get; }
        public ItemDisplayStringFunc<TItem> ItemDisplayStringFunc { get; }
        public ItemIsBoldFunc<TItem>? ItemIsBoldFunc { get; }
        public ViewModelProperty<bool> Hidden { get; set; }
        /// <summary>
        /// draw seperator or header menu 
        /// </summary>
        public bool HasMultipleLevelMenu { get; set; } = false;

        /// <summary>
        /// Create a ComboBoxOption. 
        /// </summary>
        /// <param name="property">current value; if the property Value is null, it means nothing is selected</param>
        /// <param name="itemsProperty">list of items to show</param>
        /// <param name="itemDisplayStringFunc">function to map item to display string shown in UI; if null ToString is used</param>
        /// <param name="itemIsBoldFunc">function to map item to whether or not it should be bold; bold items are currently only supported when HasMultipleLevelMenu is true</param>
        public ComboBoxOption(ViewModelProperty<TItem?> property,
            ViewModelProperty<ImmutableArray<TItem>> itemsProperty,
            ItemDisplayStringFunc<TItem>? itemDisplayStringFunc = null,
            ItemIsBoldFunc<TItem>? itemIsBoldFunc = null)
        {
            Property = property;
            ItemsProperty = itemsProperty;
            Platform = OptionFactoryPlatform.Instance.CreateComboBoxOptionPlatform(this);

            if (itemDisplayStringFunc == null)
                ItemDisplayStringFunc = DisplayableItemsUtil.ItemDisplayStringFromToString;
            else ItemDisplayStringFunc = itemDisplayStringFunc;

            ItemIsBoldFunc = itemIsBoldFunc;
        }

        /// <summary>
        /// first leve menu just show no respond action
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string CreateHeaderMenu(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return item;

            return string.Format("*{0}*", item.Trim());
        }

        public bool IsHeaderMenu(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return false;

            item = item.Trim();

            return item.StartsWith("*") && item.EndsWith("*");
        }

        public string GetHeaderMenuValue(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return item;

            return item.Trim('*').Trim();
        }

        public string CreateSeperator()
        {
            return "-";
        }

        public bool IsSeperator(string item)
        {
            if (string.IsNullOrWhiteSpace(item))
                return false;

            item = item.Trim();

            return item.Equals("-");
        }
    }
}
