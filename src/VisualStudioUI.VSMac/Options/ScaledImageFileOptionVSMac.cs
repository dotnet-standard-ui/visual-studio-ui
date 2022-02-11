using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.VisualStudioUI.Options;
using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.VSMac.Options
{
    public class ScaledImageFileOptionVSMac : OptionVSMac
    {
        private NSStackView _frameView;
        private NSStackView _view;
        private const float Space = 10f;

        public ScaledImageFileOptionVSMac(ScaledImageFileOption option) : base(option)
        {
        }

        public ScaledImageFileOption ImageOption => ((ScaledImageFileOption) Option);

        public override NSView View
        {
            get
            {
                if (_view == null)
                {
                    _view = new NSStackView(){ Orientation = NSUserInterfaceLayoutOrientation.Horizontal, Alignment = NSLayoutAttribute.CenterY };
                    _view.WantsLayer = true;
                    _view.TranslatesAutoresizingMaskIntoConstraints = false;
                    float viewWidth = (ImageOption.DrawSize + Space) * ImageOption.ImageArray.Value.Length;
                    _view.WidthAnchor.ConstraintEqualTo(viewWidth).Active = true;

                    foreach (var item in ImageOption.ImageArray.Value)
                    {
                        CreateImageView(item);
                    }
                }

                return _view!;
            }
        }

        private void CreateImageView(ScaledImageFile imageFile)
        {
            _frameView = new NSStackView()
            {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Spacing = Space,
                Distribution = NSStackViewDistribution.Fill,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // image viewer
            NSButton imageView = new NSButton ()
            {
                WantsLayer = true,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            SetAccessibilityTitleToLabel(imageView);
            imageView.Layer.BorderColor = NSColor.LightGray.CGColor;
            imageView.Layer.BorderWidth = 1f;
            imageView.Layer.CornerRadius = 4f;
            imageView.Layer.BackgroundColor = NSColor.White.CGColor;
            imageView.Activated += OnImageViewerClicked;
            imageView.Tag = -1;

            // dashed border & center label
            var border= DrawEmptyLayer(imageView, imageFile);

            _frameView.AddArrangedSubview(imageView);
            imageView.WidthAnchor.ConstraintEqualTo(ImageOption.DrawSize).Active = true;
            imageView.HeightAnchor.ConstraintEqualTo(ImageOption.DrawSize).Active = true;
            if (!string.IsNullOrWhiteSpace(imageFile.Path) && File.Exists(imageFile.Path))
            {
                var image = new NSImage(imageFile.Path ?? "");
                image.Size = new CGSize(ImageOption.DrawSize, ImageOption.DrawSize);
                imageView.Image = image;
                border?.RemoveFromSuperLayer();

                // right-menu
                if (!string.IsNullOrWhiteSpace(ImageOption.MenuLabel))
                {
                    int tag = ImageOption.ImageArray.Value.IndexOf(imageFile);
                    imageView.Tag = tag;
                    imageView.Menu = CreateMenu(tag);
                }
            }

            if (!string.IsNullOrWhiteSpace(imageFile.Hint))
            {
                imageView.ToolTip = imageFile.Hint;
            }

            // bottom label
            var bottomLabel = new NSTextField();
            bottomLabel.Editable = false;
            bottomLabel.Bordered = false;
            bottomLabel.DrawsBackground = false;
            bottomLabel.StringValue = imageFile.Label ?? string.Empty;
            bottomLabel.Alignment = NSTextAlignment.Center;
            bottomLabel.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
            bottomLabel.TextColor = NSColor.LabelColor;
            bottomLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomLabel.Tag = -1;

            _frameView.AddArrangedSubview(bottomLabel);
            
            _view.AddArrangedSubview(_frameView);
        }

        private void OnImageViewerClicked(object sender, EventArgs e)
        {
            NSButton imageViewer = (NSButton)sender;
            string path = RedrawImage(imageViewer).Trim();
            if (!string.IsNullOrWhiteSpace(path))
            {
                NSImage imageNew = new NSImage(path);
                imageNew.Size = new CGSize(ImageOption.DrawSize, ImageOption.DrawSize);
                imageViewer.Image = imageNew;

                // Remove border
                var layer = imageViewer.Layer?.Sublayers;
                if (layer != null && layer.Any())
                {
                    layer.First().RemoveFromSuperLayer();
                }

                // Update property
                List<ScaledImageFile> list = new List<ScaledImageFile>();
                int index = -1;
                for (int i = 0; i < ImageOption.ImageArray.Value.Length; i++)
                {
                    var item = ImageOption.ImageArray.Value[i];

                    if ((string.IsNullOrWhiteSpace(imageViewer?.Title) && imageViewer?.Tag == i) ||
                        (ImageOption.GetImageTitle(item).Equals(imageViewer?.Title)))
                    {
                        item.Path = path;
                        index = i;
                    }

                    list.Add(item);
                }

                ImageOption.ImageArray.Value.Clear();
                ImageOption.ImageArray.Value = ImmutableArray.CreateRange(list);

                // Create menu
                if (!string.IsNullOrWhiteSpace(ImageOption.MenuLabel))
                {
                    imageViewer.Tag = index;
                    imageViewer.Menu = CreateMenu(index);
                }

                // Remove title
                imageViewer.Title = string.Empty;
            }
        }

        private NSMenu CreateMenu(int tag)
        {
            NSMenu groupMenu = new NSMenu();
            groupMenu.AutoEnablesItems = false;

            NSMenuItem menuItem = new NSMenuItem();
            menuItem.Title = ImageOption.MenuLabel;
            menuItem.Tag = tag;
            menuItem.Activated += (sender, e) =>
            {
                ImageOption.UnsetImage(sender, e);

                foreach (var item in _view.Subviews)
                {
                    foreach (var subItem in item.Subviews)
                    {
                        if (menuItem.Tag == subItem.Tag)
                        {
                            var imageView = ((NSButton)subItem);
                            imageView.Image = null;
                            imageView.Menu?.RemoveAllItems();

                            // dashed border & center label
                            DrawEmptyLayer(imageView, ImageOption.ImageArray.Value.ElementAt(tag));

                            return;
                        }
                    }
                }
            };

            groupMenu.AddItem(menuItem);

            return groupMenu;
        }

        private CAShapeLayer DrawEmptyLayer(NSButton imageView, ScaledImageFile imageFile)
        {
            CAShapeLayer border = new CAShapeLayer();
            border.Position = new CGPoint(imageView.Bounds.X, imageView.Bounds.Y);
            CGPath path = new CGPath();
            float start = 10f;
            float width = (ImageOption.DrawSize - start * 2);
            CGRect pathRect = new CGRect(start, start, width, width);
            path.AddRect(pathRect);
            border.Path = path;
            border.LineWidth = 2;
            border.LineDashPattern = new NSNumber[] { 10, 3 };
            border.FillColor = NSColor.Clear.CGColor;
            border.StrokeColor = NSColor.LightGray.CGColor;
            imageView.Layer?.AddSublayer(border);

            // center label
            imageView.Title = ImageOption.GetImageTitle(imageFile);
            NSAttributedString attr = new NSAttributedString(imageView.Title, foregroundColor: NSColor.Gray);
            imageView.AttributedTitle = attr;

            return border;
        }

        string RedrawImage(NSButton imageViewer)
        {
            var openPanel = new NSOpenPanel();
            openPanel.CanChooseFiles = true;
            openPanel.ExtensionHidden = true;
            openPanel.AllowedFileTypes = new[] { "png" };
            var response = openPanel.RunModal();
            if (response == 1 && openPanel.Url != null)
            {
                if (File.Exists(openPanel.Url.Path))
                {
                    ScaledImageFile imageFileOld = ImageOption.GetImageFile(imageViewer.Title) ?? ImageOption.ImageArray.Value.ElementAt((int)imageViewer.Tag);

                    NSImage imageNew = new NSImage(openPanel.Url.Path);

                    if (imageNew.CGImage.Width != imageFileOld.Width || imageNew.CGImage.Height != imageFileOld.Height)
                    {
                        NSAlert alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Critical;
                        alert.MessageText = "Incorrect image dimensions";
                        alert.InformativeText = string.Format("Only images with size {0}x{1} are allowed. Picture was {2}x{3}.", imageFileOld.Width, imageFileOld.Height, imageNew.CGImage.Width, imageNew.CGImage.Height);
                        alert.RunSheetModal(null);

                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                if (imageViewer.Image != null)
                {
                    NSAlert alert = new NSAlert();
                    alert.AlertStyle = NSAlertStyle.Informational;
                    alert.AddButton("No");
                    alert.AddButton("Yes");
                    alert.Icon = NSImage.GetSystemSymbol("info.circle", null);
                    alert.MessageText = "Image already exists";
                    alert.InformativeText = string.Format("Should {0} be overwritten?", imageViewer.Title);
                    var result = alert.RunSheetModal(null);
                    if (result == (int)NSAlertButtonReturn.First)
                    {
                        return string.Empty;
                    }
                }

                return openPanel.Url.Path;
            }

            return string.Empty;
        }
    }
}
