using System;
using System.Collections.Immutable;
using AppKit;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class ComboBoxOptionVSMac<TItem> : OptionWithLeftLabelVSMac where TItem : class
    {
        private NSPopUpButton _popUpButton;

        public bool NullIsDefault { get; set; } = false;

        public ComboBoxOptionVSMac(ComboBoxOption<TItem> option) : base(option)
        {
        }

        public ComboBoxOption<TItem> ComboBoxOption => ((ComboBoxOption<TItem>)Option);

        protected override NSView ControlView
        {
            get
            {
                if (_popUpButton == null)
                {
                    ViewModelProperty<TItem?> property = ComboBoxOption.Property;
                    ViewModelProperty<ImmutableArray<TItem>> itemsProperty = ComboBoxOption.ItemsProperty;

                    _popUpButton = new NSPopUpButton
                    {
                        BezelStyle = NSBezelStyle.Rounded,
                        ControlSize = NSControlSize.Regular,
                        Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize),
                        TranslatesAutoresizingMaskIntoConstraints = false
                    };
                    SetAccessibilityTitleToLabel(_popUpButton);

                    _popUpButton.WidthAnchor.ConstraintEqualToConstant(198f).Active = true;

                    _popUpButton.Activated += UpdatePropertyFromUI;

                    if (ComboBoxOption.Hidden != null)
                    {
                        ComboBoxOption.Hidden.PropertyChanged += HidView;
                    }

                    property.PropertyChanged += delegate { UpdateSelectedItemUIFromProperty(); };

                    if (ComboBoxOption.HasMultipleLevelMenu)
                    {
                        itemsProperty.PropertyChanged += delegate { UpdateMultipleLevelMenuItemChoices(); };
                        UpdateMultipleLevelMenuItemChoices();
                    }
                    else
                    {
                        itemsProperty.PropertyChanged += delegate {
                            UpdateItemChoices();
                            UpdateHintButton();
                        };

                        UpdateItemChoices();
                    }
                }

                return _popUpButton;
            }
        }


        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);

            if (_popUpButton != null)
                _popUpButton.Enabled = enabled;
        }

        private void UpdatePropertyFromUI(object sender, EventArgs e)
        {

            int selectedIndex = (int)_popUpButton.IndexOfSelectedItem;
            TItem? match = ComboBoxOption.ItemsProperty.Value[selectedIndex];
            ComboBoxOption.Property.Value = match;
        }

        private void UpdateItemChoices()
        {
            _popUpButton!.RemoveAllItems();

            ImmutableArray<TItem> items = ComboBoxOption.ItemsProperty.Value;

            // The intention is that null items aren't allowed - no items should be an empty list.
            // But handle this case just in case, to be safe.
            if ((ImmutableArray<TItem>?)items == null)
                return;

            foreach (TItem item in items)
            {
                string itemDisplayString = ComboBoxOption.ItemDisplayStringFunc(item);
                _popUpButton.AddItem(itemDisplayString);
            }

            UpdateSelectedItemUIFromProperty();
        }

        private void UpdateMultipleLevelMenuItemChoices()
        {
            _popUpButton!.RemoveAllItems();

            NSMenu groupMenu = new NSMenu();
            groupMenu.AutoEnablesItems = false;
            bool hasIndentate = false;

            foreach (var item in ComboBoxOption.ItemsProperty.Value)
            {
                string itemDisplayString = ComboBoxOption.ItemDisplayStringFunc(item);

                if (ComboBoxOption.IsSeperator(itemDisplayString))
                {
                    groupMenu.AddItem(NSMenuItem.SeparatorItem);
                    hasIndentate = false;
                    continue;
                }

                NSMenuItem menuItem = new NSMenuItem();

                if (ComboBoxOption.IsHeaderMenu(itemDisplayString))
                {
                    //header
                    itemDisplayString = ComboBoxOption.GetHeaderMenuValue(itemDisplayString);
                    menuItem.Enabled = false;
                    hasIndentate = true;
                }
                else if (hasIndentate)
                {
                    menuItem.IndentationLevel = 1;
                }

                menuItem.Title = itemDisplayString;

                groupMenu.AddItem(menuItem);
            }

            _popUpButton.Menu = groupMenu;
            _popUpButton.TranslatesAutoresizingMaskIntoConstraints = false;

            UpdateSelectedItemUIFromProperty();
        }

        private void UpdateSelectedItemUIFromProperty()
        {
            TItem? currentValue = ComboBoxOption.Property.Value;
            if (currentValue == null)
                _popUpButton!.SelectItem((NSMenuItem?)null);
            else
            {
                //string currenValueDisplayString = ComboBoxOption.ItemDisplayStringFunc(currentValue);
                //_popUpButton!.SelectItem(currenValueDisplayString);
                int itemIndex = ComboBoxOption.ItemsProperty.Value.IndexOf(currentValue);
                _popUpButton.SelectItem(itemIndex);
            }
        }

        private void HidView(object sender, ViewModelPropertyChangedEventArgs e)
        {
            _popUpButton.Hidden = ComboBoxOption.Hidden.Value;
        }

    }
}