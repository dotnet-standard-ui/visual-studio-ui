using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using AppKit;
using Foundation;
using Microsoft.VisualStudioUI.Options;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class EnvironmentVariableOptionVSMac : OptionVSMac
    {
        internal const string VariablesColumnId = "VariablesColumnId";
        internal const string ValuesColumnId = "ValuesColumnId";

        internal List<string> Variables;
        internal List<string> Values;
        private NSView _optionView;
        private NSTableView _tableView;
        private NSButton _addButton, _removeButton;

        private readonly List<string> _stringList = new List<string>();

        public EnvironmentVariableOptionVSMac(EnvironmentVariableOption option) : base(option)
        {
            Variables = new List<string>();
            Values = new List<string>();
        }

        private void UpdateStringListFromModel()
        {
            _stringList.Clear();
            foreach (string item in EnvironmentVariableOption.Property.Value)
            {
                _stringList.Add(item);
            }
        }

        private void UpdateModelFromStringList()
        {
            EnvironmentVariableOption.Property.PropertyChanged -= OnStringsListChanged;
            EnvironmentVariableOption.Property.Value = _stringList.ToImmutableArray();
            EnvironmentVariableOption.Property.PropertyChanged += OnStringsListChanged;
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

        public EnvironmentVariableOption EnvironmentVariableOption => ((EnvironmentVariableOption)Option);

        public IntPtr Handle => throw new NotImplementedException();

        public void CreateView()
        {
            _optionView = new NSView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _tableView = new NSTableView()
            {
                HeaderView = null,
                Source = new VariableSource(this),
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            _tableView.AddColumn(new NSTableColumn(VariablesColumnId) { Title = EnvironmentVariableOption.VariablesColumnTitle});
            _tableView.AddColumn(new NSTableColumn(ValuesColumnId) { Title = EnvironmentVariableOption.ValuesColumnTitle });

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
                Title = EnvironmentVariableOption.AddButtonTitle,
                ToolTip = EnvironmentVariableOption.AddToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _addButton.Activated += OnAddClicked;

            _removeButton = new NSButton
            {
                BezelStyle = NSBezelStyle.RoundRect,
                Title = EnvironmentVariableOption.RemoveButtonTitle,
                ToolTip = EnvironmentVariableOption.RemoveToolTip,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _removeButton.Activated += OnRemoveClicked;
            _optionView.AddSubview(_addButton);
            _optionView.AddSubview(_removeButton);

            if (!string.IsNullOrEmpty(Option.Label))
            {
                var left = new NSTextField();
                left.Editable = false;
                left.Bordered = false;
                left.DrawsBackground = false;
                left.StringValue = Option.Label + ":";
                left.Alignment = NSTextAlignment.Right;
                left.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                left.TextColor = NSColor.LabelColor;
                left.TranslatesAutoresizingMaskIntoConstraints = false;
                left.SizeToFit();
                _optionView.AddSubview(left);
                left.LeadingAnchor.ConstraintEqualToAnchor(_optionView.LeadingAnchor, IndentValue()).Active = true;
                left.WidthAnchor.ConstraintEqualToConstant(205).Active = true;
                left.BottomAnchor.ConstraintEqualToAnchor(_optionView.TopAnchor).Active = true;
            }

            _addButton.WidthAnchor.ConstraintEqualToConstant(30).Active = true;
            _addButton.HeightAnchor.ConstraintEqualToConstant(25).Active = true;
            _removeButton.WidthAnchor.ConstraintEqualToConstant(30).Active = true;
            _removeButton.HeightAnchor.ConstraintEqualToConstant(25).Active = true;
            scrolledView.HeightAnchor.ConstraintEqualToConstant(200).Active = true;
            scrolledView.WidthAnchor.ConstraintEqualToConstant(374).Active = true;
            _optionView.WidthAnchor.ConstraintEqualToConstant(640f).Active = true;

            _addButton.TopAnchor.ConstraintEqualToAnchor(scrolledView.BottomAnchor, 10).Active = true;
            _addButton.LeadingAnchor.ConstraintEqualToAnchor(scrolledView.LeadingAnchor).Active = true;
            _removeButton.TopAnchor.ConstraintEqualToAnchor(_addButton.TopAnchor).Active = true;
            _removeButton.LeadingAnchor.ConstraintEqualToAnchor(_addButton.TrailingAnchor, 10).Active = true;
            _optionView.BottomAnchor.ConstraintEqualToAnchor(_addButton.BottomAnchor, 2).Active = true;
            scrolledView.TopAnchor.ConstraintEqualToAnchor(_optionView.TopAnchor).Active = true;

          
            scrolledView.LeadingAnchor.ConstraintEqualToAnchor(_optionView.LeadingAnchor, 20 + IndentValue()).Active = true;

            EnvironmentVariableOption.Property.PropertyChanged += OnStringsListChanged;

            UpdateStringListFromModel();
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

        private void OnStringsListChanged(object sender, EventArgs e)
        {
            UpdateStringListFromModel();

            _tableView.ReloadData();

            TableSelectLastItem();

            _removeButton.Enabled = _stringList.Count > 0;

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
            if (string.IsNullOrEmpty(newText) || newText.Equals(_stringList[row]))
            {
                return;
            }

            string? newValue =  newText;

            _stringList[row] = newValue;

            UpdateModelFromStringList();
            RefreshList();
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            var defalutString = string.Empty;

            try
            {
                _stringList.Add(defalutString);
                UpdateModelFromStringList();
                RefreshList();
            }
            catch
            {
                return;
            }
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            int selectedRow = (int)_tableView.SelectedRow;

            if (selectedRow < 0 || selectedRow >= _stringList.Count)
            {
                return;
            }
            //fix reference bug when selected row in textfield editing state
            NSTableRowView? view = _tableView.GetRowView(selectedRow, false);
            view.Window.MakeFirstResponder(null);

            try
            {
                _stringList.RemoveAt(selectedRow);
                UpdateModelFromStringList();
                RefreshList();
            }
            catch
            {
                return;
            }
        }

        private void TableSelectLastItem()
        {
            if (_stringList.Count <= 0)
            {
                return;
            }
            int selectRow = _stringList.Count - 1;

            _tableView.SelectRow(selectRow, false);
            _tableView.ScrollRowToVisible(selectRow);
        }


        private void RefreshList()
        {
            _tableView.ReloadData();

            TableSelectLastItem();

            _removeButton.Enabled = _stringList.Count > 0;

        }
    }

    internal class VariableSource : NSTableViewSource
    {
        private readonly EnvironmentVariableOptionVSMac _platform;

        public VariableSource(EnvironmentVariableOptionVSMac platform)
        {
            _platform = platform;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
    
            var view = (NSTableCellView)tableView.MakeView("cell", this);
            if (view == null)
            {
                view = new NSTableCellView
                {
                    TextField = new NSTextField
                    {
                        Frame = new CoreGraphics.CGRect(5, 2, tableColumn.Width, 20),
                        Hidden = false,
                        Bordered = false,
                        DrawsBackground = false,
                        Highlighted = false,
                    },
                    Identifier = "cell"
                };

                view.AddSubview(view.TextField);

                view.TextField.EditingEnded += _platform.OnValueEdited;
            }

            view.TextField.Tag = row;

            string value = string.Empty;

            if (tableColumn.Identifier == EnvironmentVariableOptionVSMac.VariablesColumnId)
            {
                value = _platform.Variables[(int)row];
            }
            else if(tableColumn.Identifier == EnvironmentVariableOptionVSMac.ValuesColumnId)
            {
                value = _platform.Values[(int)row];
            }

            view.TextField.StringValue = value;

            return view;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return _platform.Variables.Count;
        }
    }

}
