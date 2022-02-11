// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AppKit;

using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{

    public class CheckBoxListOptionVSMac : OptionVSMac
    {
        private NSView _optionView;
        private NSTableView _tableView;
        private NSButton _upButton, _downButton;

        public CheckBoxListOptionVSMac(CheckBoxListOption option) : base(option)
        {
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

        public CheckBoxListOption CheckBoxListOption => ((CheckBoxListOption)Option);

        public IntPtr Handle => throw new NotImplementedException();

        public void CreateView()
        {
            _optionView = new NSView();
                                     
            _optionView.TranslatesAutoresizingMaskIntoConstraints = false;

            _tableView = new NSTableView()
            {
                HeaderView = null,
                Source = new CheckBoxSource(this),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _tableView.GridStyleMask = NSTableViewGridStyle.DashedHorizontalGridLine;
            _tableView.AddColumn(new NSTableColumn());
            if (CheckBoxListOption.AllowReordering)
                _tableView.Activated += OnSelectionChanged;
            else
                _tableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;

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

            if (CheckBoxListOption.AllowReordering)
            {
                _upButton = new NSButton
                {
                    BezelStyle = NSBezelStyle.RoundRect,
                    ControlSize = NSControlSize.Regular,
                    Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize),
                    Title = "Move up",
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    ToolTip = CheckBoxListOption.UpToolTip,
                    Enabled = false,
                };
                _upButton.Activated += OnMoveUpClicked;
                _upButton.SizeToFit();

                _downButton = new NSButton
                {
                    BezelStyle = NSBezelStyle.RoundRect,
                    ControlSize = NSControlSize.Regular,
                    Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize),
                    Title = "Move down",
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    ToolTip = CheckBoxListOption.DownToolTip,
                    Enabled = false,
                };
                _downButton.Activated += OnMoveDownClicked;
                _downButton.SizeToFit();

                _optionView.AddSubview(_upButton);
                _optionView.AddSubview(_downButton);
            }

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
                left.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor).Active = true;
                left.WidthAnchor.ConstraintEqualTo(205).Active = true;
                left.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor).Active = true;
            }

            if (CheckBoxListOption.AllowReordering)
            {
                if (_upButton.Title.Length > _downButton.Title.Length)
                    _downButton.WidthAnchor.ConstraintEqualTo(_upButton.WidthAnchor).Active = true;
                else
                    _upButton.WidthAnchor.ConstraintEqualTo(_downButton.WidthAnchor).Active = true;
            }

            _optionView.WidthAnchor.ConstraintEqualTo(640f - IndentValue()).Active = true;

            scrolledView.HeightAnchor.ConstraintEqualTo(CheckBoxListOption.Height).Active = true;
            scrolledView.WidthAnchor.ConstraintEqualTo(CheckBoxListOption.Width).Active = true;

            if (CheckBoxListOption.AllowReordering)
            {
                _upButton.TopAnchor.ConstraintEqualTo(scrolledView.BottomAnchor, 10).Active = true;
                _upButton.LeadingAnchor.ConstraintEqualTo(scrolledView.LeadingAnchor).Active = true;
                _downButton.TopAnchor.ConstraintEqualTo(_upButton.TopAnchor).Active = true;
                _downButton.LeadingAnchor.ConstraintEqualTo(_upButton.TrailingAnchor, 10).Active = true;
                _optionView.BottomAnchor.ConstraintEqualTo(_upButton.BottomAnchor, 2).Active = true;
            }

            scrolledView.TopAnchor.ConstraintEqualTo(_optionView.TopAnchor).Active = true;

            float leftSpace = Option.AllowSpaceForLabel ? 222f:20f;
            scrolledView.LeadingAnchor.ConstraintEqualTo(_optionView.LeadingAnchor, leftSpace + IndentValue()).Active = true;
            if (!CheckBoxListOption.AllowReordering)
            {
                scrolledView.BottomAnchor.ConstraintEqualTo(_optionView.BottomAnchor).Active = true;
            }

            CheckBoxListOption.Property.PropertyChanged += OnCheckBoxListChanged;
        }

        public bool ShowDescriptions
        {
            get { return false; }
        }

        public override void OnEnableChanged(bool enabled)
        {
            base.OnEnableChanged(enabled);

            if (CheckBoxListOption.AllowReordering)
            {
                _upButton.Enabled = enabled;
                _downButton.Enabled = enabled;
            }
            _tableView.Enabled = enabled;
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonsStatus();
        }

        private void UpdateButtonsStatus()
        {
            _upButton.Enabled = (_tableView.SelectedRow > 0);
            _downButton.Enabled = (_tableView.SelectedRow < CheckBoxListOption.Property.Value.Length - 1);
        }

        private void OnCheckBoxListChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        public void OnItemToggled(object sender, EventArgs e)
        {

            var checkBoxButton = (NSButton)(sender);

            int row = (int)checkBoxButton.Tag;

            CheckBoxListOption.Property.Value[row].Selected = (checkBoxButton.State == NSCellStateValue.On);
            CheckBoxListOption.ListChangedInvoke(sender, e);
        }

        private void RefreshList(int selectIndex = -1)
        {
            _tableView.ReloadData();

            if (CheckBoxListOption.AllowReordering)
            {
                if (selectIndex > -1)
                {
                    _tableView.SelectRow(selectIndex, false);
                }

                UpdateButtonsStatus();
            }
        }

        private void OnMoveUpClicked(object sender, EventArgs e)
        {
            if (CheckBoxListOption.Property.Value.Any())
            {
                int selectedIndex = (int)_tableView.SelectedRow;
                if (selectedIndex <= 0) return;// selected the first one

                List<CheckBoxlistItem> list = new List<CheckBoxlistItem>(CheckBoxListOption.Property.Value);

                var old = list.ElementAt(selectedIndex - 1);
                list[selectedIndex - 1] = list.ElementAt(selectedIndex);
                list[selectedIndex] = old;

                CheckBoxListOption.Property.Value = CheckBoxListOption.Property.Value.Clear();
                CheckBoxListOption.Property.Value = ImmutableArray.CreateRange(list);

                RefreshList(selectedIndex - 1);

                CheckBoxListOption.ListChangedInvoke(sender, e);
            }
        }

        private void OnMoveDownClicked(object sender, EventArgs e)
        {
            if (CheckBoxListOption.Property.Value.Any())
            {
                int selectedIndex = (int)_tableView.SelectedRow;
                List<CheckBoxlistItem> list = new List<CheckBoxlistItem>(CheckBoxListOption.Property.Value);
                if (selectedIndex >= list.Count - 1) return; // selected the last one

                var old = list.ElementAt(selectedIndex + 1);
                list[selectedIndex + 1] = list.ElementAt(selectedIndex);
                list[selectedIndex] = old;

                CheckBoxListOption.Property.Value = CheckBoxListOption.Property.Value.Clear();
                CheckBoxListOption.Property.Value = ImmutableArray.CreateRange(list);

                RefreshList(selectedIndex + 1);

                CheckBoxListOption.ListChangedInvoke(sender, e);
            }
        }
    }

    internal class CheckBoxSource : NSTableViewSource
    {
        private readonly CheckBoxListOptionVSMac _platform;

        public CheckBoxSource(CheckBoxListOptionVSMac platform)
        {
            _platform = platform;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            const string identifer = "cell";

            var view = (NSButton)tableView.MakeView(identifer, this);
            if (view == null)
            {
                view = new NSButton();
                view.Bordered = false;
                view.SetButtonType(NSButtonType.Switch);
                view.Activated += _platform.OnItemToggled;
                view.Identifier = identifer;
            }

            view.Tag = row;
            view.Title = _platform.CheckBoxListOption.Property.Value[(int)row].Title;
            view.State = _platform.CheckBoxListOption.Property.Value[(int)row].Selected ? NSCellStateValue.On : NSCellStateValue.Off;

            return view;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return _platform.CheckBoxListOption.Property.Value.Length;
        }

        //public override nfloat GetRowHeight(NSTableView tableView, nint row)
        //{
        //	return 30;
        //}
    }

}
