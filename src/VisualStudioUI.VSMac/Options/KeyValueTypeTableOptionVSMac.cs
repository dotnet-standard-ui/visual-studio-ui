using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AppKit;
using Foundation;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class KeyValueTypeTableOptionVSMac : OptionVSMac
    {
        internal const string KeyColumnId = "KeyColumnId";
        internal const string TypeColumnId = "TypeColumnId";
        internal const string ValueColumnId = "ValueColumnId";
        internal List<KeyValueItem> Items;

        private NSView _optionView;
        private NSTableView _tableView;
        private NSButton _addButton, _removeButton, _editButton;

        public KeyValueTypeTableOptionVSMac(KeyValueTypeTableOption option) : base(option)
        {
            Items = new List<KeyValueItem>();
        }

        private void UpdateListFromModel()
        {
            Items.Clear();
            if (KeyValueTypeTableOption.Property.Value == null)
            {
                return;
            }
            foreach (var item in KeyValueTypeTableOption.Property.Value)
            {
                Items.Add(item);
            }
            RefreshList();
        }

        private void UpdateModelFromList()
        {
            KeyValueTypeTableOption.Property.PropertyChanged -= OnListChanged;
            KeyValueTypeTableOption.Property.Value = Items.ToImmutableArray();
            KeyValueTypeTableOption.Property.PropertyChanged += OnListChanged;
        }

        public override NSView View
        {
            get
            {
                if (_optionView == null)
                {
                    CreateView();
                }

                return _optionView;
            }
        }

        public KeyValueTypeTableOption KeyValueTypeTableOption => ((KeyValueTypeTableOption)Option);

        public IntPtr Handle => throw new NotImplementedException();

        public void CreateView()
        {
            _optionView = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _tableView = new NSTableView()
            {
                Source = new KeyValueTypeTableSource(this),
                AllowsColumnReordering = false,
                AllowsMultipleSelection = false,
                SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _tableView.AddColumn(new NSTableColumn(KeyColumnId)
            {
                Title = KeyValueTypeTableOption.KeyColumnTitle,
            });

            _tableView.AddColumn(new NSTableColumn(TypeColumnId)
            {
                Title = KeyValueTypeTableOption.TypeColumnTitle,
            });

            _tableView.AddColumn(new NSTableColumn(ValueColumnId)
            {
                Title = KeyValueTypeTableOption.ValueColumnTitle,
            });

            var scrolledView = new NSScrollView()
            {
                DocumentView = _tableView,
                BorderType = NSBorderType.LineBorder,
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                AutohidesScrollers = true,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _optionView.AddSubview(scrolledView);
            _addButton = new NSButton
            {
                BezelStyle = NSBezelStyle.RoundRect,
                Title = KeyValueTypeTableOption.AddButtonTitle,
                ToolTip = KeyValueTypeTableOption.AddToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _addButton.SizeToFit();
            _addButton.Activated += OnAddClicked;

            _removeButton = new NSButton
            {
                BezelStyle = NSBezelStyle.RoundRect,
                Title = KeyValueTypeTableOption.RemoveButtonTitle,
                ToolTip = KeyValueTypeTableOption.RemoveToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _removeButton.SizeToFit();
            _removeButton.Activated += OnRemoveClicked;

            _editButton = new NSButton
            {
                BezelStyle = NSBezelStyle.RoundRect,
                Title = KeyValueTypeTableOption.EditButtonTitle,
                ToolTip = KeyValueTypeTableOption.EditToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _editButton.SizeToFit();
            _editButton.Activated += OnEditClicked;

            _optionView.AddSubview(_addButton);
            _optionView.AddSubview(_removeButton);
            _optionView.AddSubview(_editButton);

            _optionView.AddSubview(_addButton);
            _optionView.AddSubview(_removeButton);
            if (!string.IsNullOrEmpty(Option.Label))
            {
                var title = new NSTextField();
                title.Editable = false;
                title.Bordered = false;
                title.DrawsBackground = false;
                title.StringValue = Option.Label + ":";
                title.Alignment = NSTextAlignment.Left;
                title.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                title.TextColor = NSColor.LabelColor;
                title.TranslatesAutoresizingMaskIntoConstraints = false;
                title.SizeToFit();
                _optionView.AddSubview(title);
                title.LeadingAnchor.ConstraintEqualToAnchor(scrolledView.LeadingAnchor, IndentValue()).Active = true;
                title.WidthAnchor.ConstraintEqualToConstant(205).Active = true;
                title.BottomAnchor.ConstraintEqualToAnchor(scrolledView.TopAnchor, -2).Active = true;
            }
            scrolledView.HeightAnchor.ConstraintEqualToConstant(200f).Active = true;
            scrolledView.WidthAnchor.ConstraintEqualToConstant(560f).Active = true;
            _optionView.WidthAnchor.ConstraintEqualToConstant(640f).Active = true;

            _addButton.TopAnchor.ConstraintEqualToAnchor(scrolledView.BottomAnchor, 10).Active = true;
            _addButton.LeadingAnchor.ConstraintEqualToAnchor(scrolledView.LeadingAnchor).Active = true;
            _removeButton.TopAnchor.ConstraintEqualToAnchor(_addButton.TopAnchor).Active = true;
            _removeButton.LeadingAnchor.ConstraintEqualToAnchor(_addButton.TrailingAnchor, 10).Active = true;
            _editButton.TopAnchor.ConstraintEqualToAnchor(scrolledView.BottomAnchor, 10).Active = true;
            _editButton.TrailingAnchor.ConstraintEqualToAnchor(scrolledView.TrailingAnchor).Active = true;

            _optionView.BottomAnchor.ConstraintEqualToAnchor(_addButton.BottomAnchor, 2).Active = true;
            scrolledView.TopAnchor.ConstraintEqualToAnchor(_optionView.TopAnchor, 25f).Active = true;
            scrolledView.LeadingAnchor.ConstraintEqualToAnchor(_optionView.LeadingAnchor, 20 + IndentValue()).Active = true;

            KeyValueTypeTableOption.Property.PropertyChanged += OnListChanged;
            KeyValueTypeTableOption.SelectedProperty.PropertyChanged += OnSelectionChanged;
            UpdateListFromModel();
        }

        public override void OnEnableChanged(bool enabled)
        {
            _addButton.Enabled = enabled;
            _removeButton.Enabled = enabled;
            _tableView.Enabled = enabled;
        }

        public bool ShowDescriptions
        {
            get { return false; }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonEnable();
        }

        internal void UpdateButtonEnable()
        {
            _editButton.Enabled = _tableView.SelectedRow != -1;
            _removeButton.Enabled = _editButton.Enabled;
        }

        private void OnListChanged(object sender, EventArgs e)
        {
            UpdateListFromModel();

            _tableView.ReloadData();

            UpdateButtonEnable();

        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            KeyValueTypeTableOption.AddInvoke(sender, e);
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            KeyValueTypeTableOption.RemoveInvoke(sender, e);
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            KeyValueTypeTableOption.EditInvoke(sender, e);
        }

        private void RefreshList()
        {
            _tableView.ReloadData();
            UpdateButtonEnable();
        }
    }

    internal class KeyValueTypeTableSource : NSTableViewSource
    {
        private readonly KeyValueTypeTableOptionVSMac _platform;

        public KeyValueTypeTableSource(KeyValueTypeTableOptionVSMac platform)
        {
            _platform = platform;
        }

        public override bool ShouldSelectRow(NSTableView tableView, nint row)
        {
            _platform.KeyValueTypeTableOption.SelectedProperty.Value = _platform.Items[(int)row];
            return true;
        }

        public override void SelectionDidChange(NSNotification notification)
        {
            _platform.UpdateButtonEnable();
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {

            var view = (NSTableCellView)tableView.MakeView(tableColumn.Identifier, this);
            if (view == null)
            {
                view = new NSTableCellView
                {
                    TextField = new NSTextField
                    {
                        Frame = new CoreGraphics.CGRect(0, 2, tableColumn.Width, 20),
                        Hidden = false,
                        Bordered = false,
                        DrawsBackground = false,
                        Editable = false,
                        Identifier = tableColumn.Identifier
                    },

                    Identifier = tableColumn.Identifier
                };
                view.AddSubview(view.TextField);
            }

            view.TextField.Tag = row;

            string value = string.Empty;

            if (tableColumn.Identifier == KeyValueTypeTableOptionVSMac.KeyColumnId)
            {
                value = _platform.Items[(int)row].Key;
            }
            else if (tableColumn.Identifier == KeyValueTypeTableOptionVSMac.ValueColumnId)
            {
                value = _platform.Items[(int)row].Value;
            }
            else if (tableColumn.Identifier == KeyValueTypeTableOptionVSMac.TypeColumnId)
            {
                value = _platform.Items[(int)row].Type;
            }

            view.TextField.StringValue = value;

            return view;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return _platform.Items.Count;
        }
    }

}
