// Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudioUI
{
    public static class VSUIElementExtensions
    {
        public static T Uri<T>(this T hyperlinkButton, string value) where T : IHyperlinkButton
        {
            hyperlinkButton.Uri = value;
            return hyperlinkButton;
        }
        public static T Label<T>(this T hyperlinkButton, string value) where T : IHyperlinkButton
        {
            hyperlinkButton.Label = value;
            return hyperlinkButton;
        }
        public static T Source<T>(this T image, string value) where T : IImage
        {
            image.Source = value;
            return image;
        }
    }
}
