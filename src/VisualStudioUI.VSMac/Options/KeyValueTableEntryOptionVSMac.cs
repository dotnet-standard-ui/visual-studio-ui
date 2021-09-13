using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AppKit;
using Foundation;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class KeyValueTableEntryOptionVSMac : OptionVSMac
    {
        internal const string KeyColumnId = "FirstColumnId";
        internal const string ValueColumnId = "SecondColumnId";
        internal List<KeyValueItem> Items;

        private NSView _optionView;
        private NSTableView _tableView;
        private NSButton _addButton, _removeButton;

        public KeyValueTableEntryOptionVSMac(KeyValueTableEntryOption option) : base(option)
        {
            Items = new List<KeyValueItem >();
        }

        private void UpdateListFromModel()
        {
            Items.Clear();
            if(KeyValueTableEntryOption.Property.Value == null)
            {
                return;
            }
            foreach (var item in KeyValueTableEntryOption.Property.Value)
            {
                Items.Add(item);
            }
        }

        private void UpdateModelFromList()
        {
            KeyValueTableEntryOption.Property.PropertyChanged -= OnListChanged;

            KeyValueTableEntryOption.Property.Value = Items.ToImmutableArray();

            KeyValueTableEntryOption.Property.PropertyChanged += OnListChanged;
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

        public KeyValueTableEntryOption KeyValueTableEntryOption => ((KeyValueTableEntryOption)Option);

        public IntPtr Handle => throw new NotImplementedException();

        public void CreateView()
        {
            _optionView = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _tableView = new NSTableView()
            {
                Source = new KeyValueTableSource(this),
                AllowsColumnReordering = false,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _tableView.AddColumn(new NSTableColumn(KeyColumnId){
                Title = KeyValueTableEntryOption.KeyColumnTitle
            });

            _tableView.AddColumn(new NSTableColumn(ValueColumnId)
            {
                Title = KeyValueTableEntryOption.ValueColumnTitle,
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
                Title = KeyValueTableEntryOption.AddButtonTitle,
                ToolTip = KeyValueTableEntryOption.AddToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _addButton.SizeToFit();
            _addButton.Activated += OnAddClicked;
            _removeButton = new NSButton
            {
                BezelStyle = NSBezelStyle.RoundRect,
                Title = KeyValueTableEntryOption.RemoveButtonTitle,
                ToolTip = KeyValueTableEntryOption.RemoveToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _removeButton.SizeToFit();
            _removeButton.Activated += OnRemoveClicked;
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

            _optionView.BottomAnchor.ConstraintEqualToAnchor(_addButton.BottomAnchor, 2).Active = true;          
            scrolledView.LeadingAnchor.ConstraintEqualToAnchor(_optionView.LeadingAnchor, 20 + IndentValue()).Active = true;
            scrolledView.TopAnchor.ConstraintEqualToAnchor(_optionView.TopAnchor, 25).Active = true;
            KeyValueTableEntryOption.Property.PropertyChanged += OnListChanged;

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
            _removeButton.Enabled = (_tableView.SelectedCell != null);

        }

        private void OnListChanged(object sender, EventArgs e)
        {
            UpdateListFromModel();

            _tableView.ReloadData();

            TableSelectLastItem();

            _removeButton.Enabled = Items.Count > 0;

        }

        public void OnValueEdited(object sender, EventArgs e)
        {
            var sObject = ((NSNotification)sender).Object;

            if (sObject == null)
            {
                return;
            }

            var textField = (NSTextField)(sObject);
            
            int row = (int)textField.Tag;
            string? newText = textField.StringValue;
            if (string.IsNullOrEmpty(newText))
            {
                return;
            }

            if (textField.Identifier.Equals(KeyColumnId))
            {
               Items[(int)row].Key = newText;
            }
            else if (textField.Identifier.Equals(ValueColumnId))
            {
               Items[(int)row].Value = newText;
            }

            UpdateModelFromList();
 
        }

        private void OnAddClicked(object sender, EventArgs e)
        {

            Items.Add(new KeyValueItem(string.Empty, string.Empty));
        
            UpdateModelFromList();
            RefreshList();
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            int selectedRow = (int)_tableView.SelectedRow;

            if (selectedRow < 0 || selectedRow >= Items.Count)
            {
                return;
            }
            //fix reference bug when selected row in textfield editing state
            NSTableRowView? view = _tableView.GetRowView(selectedRow, false);
            view.Window.MakeFirstResponder(null);
            Items.RemoveAt(selectedRow);
            UpdateModelFromList();
            RefreshList();
        }

        private void TableSelectLastItem()
        {
            if (Items.Count <= 0)
            {
                return;
            }
            int selectRow = Items.Count - 1;

            _tableView.SelectRow(selectRow, false);
            _tableView.ScrollRowToVisible(selectRow);
        }


        private void RefreshList()
        {
            _tableView.ReloadData();

            TableSelectLastItem();

            _removeButton.Enabled = Items.Count > 0;

        }
    }

    internal class KeyValueTableSource : NSTableViewSource
    {
        private readonly KeyValueTableEntryOptionVSMac _platform;

        public KeyValueTableSource(KeyValueTableEntryOptionVSMac platform)
        {
            _platform = platform;
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
                        DrawsBackground = true,
                        Highlighted = false,
                        Identifier = tableColumn.Identifier
                    },
          
                    Identifier = tableColumn.Identifier
                };

                view.AddSubview(view.TextField);

                view.TextField.EditingEnded += _platform.OnValueEdited;
            }

            view.TextField.Tag = row;

            string value = string.Empty;

            if (tableColumn.Identifier == KeyValueTableEntryOptionVSMac.KeyColumnId)
            {
                value = _platform.Items[(int)row].Key;
            }
            else if(tableColumn.Identifier == KeyValueTableEntryOptionVSMac.ValueColumnId)
            {
                value = _platform.Items[(int)row].Value;
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
