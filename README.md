# visual-studio-ui
UI objects for Visual Studio / Visual Studio for Mac UI

# Clone

1. If you are looking to use the library on existing solution, add the repo as a submodule:
```
git submodule add https://github.com/dotnet-standard-ui/visual-studio-ui.git external\visual-studio-ui
```
2. Update submodules manually:
```
git submodule update --init --recursive
```
3. Else, if you are not adding this repo as submodule, clone this repository and its submodules via your favorite git client:
```
git clone --recursive git@github.com:dotnet-standard-ui/visual-studio-ui.git
```
# Build Shared UI
1. In the project that will be shared by both VS and VS for Mac, add below projects as project reference:
    - `\src\Microsoft.VisualStudioUI\Microsoft.VisualStudioUI.csproj`
    - `\external\standard-ui\src\Microsoft.StandardUI\Microsoft.StandardUI.csproj`

2. Create a empty .cs file with desired file name you want to use for the UI Control.

3. Paste the code below, but replace all the `FileName` and the `NameSpace` with correct names:
```C#
using Microsoft.StandardUI;
using Microsoft.StandardUI.Controls;
using Microsoft.StandardUI.Shapes;
using Microsoft.VisualStudioUI;
using static Microsoft.StandardUI.StandardUIStatics;
using static Microsoft.VisualStudioUI.VisualStudioUIStatics;

namespace NameSpace
{
    [StandardControl]
    public interface IFileName : IStandardControl
    {
        Color SampleProperty { get; set; }
    }

    public class FileNameImplementation : StandardControlImplementation<IFileName>
    {
        public FileNameImplementation(IFileName control) : base(control)
        {
        }

        public override IUIElement Build() =>
            /*
             * Add UI code in this section
             */
    }
}
```
4. Fill in the UI code. Here are some resources and examples:
    - [OverviewSample]("/samples/Overview/Overview/OverviewSample.cs")
    - [VisualStudioUIGallery]("/samples/GalleryExtension/Microsoft.VisualStudioUIGallery.VSWin")


5. Once it is built, the UI will be available to be used as controls inside either Windows or Mac.

# Use in Windows (WPF)

1. In a WPF project, add below projects as project reference:
    - `\src\Microsoft.VisualStudioUI\Microsoft.VisualStudioUI.csproj`
    - `\src\Microsoft.VisualStudioUI.VSWin\Microsoft.VisualStudioUI.VSWin.csproj`
    - `\external\standard-ui\src\Microsoft.StandardUI\Microsoft.StandardUI.csproj`
    - `\external\standard-ui\src\Microsoft.StandardUI.Wpf\Microsoft.StandardUI.Wpf.csproj`
    
    It is recommended to add these projects inside the solution, or make sure that they are all built.

2. One thing that should be done is adding `Microsoft.StandardUI.SourceGenerator.dll` as Analyzer in the WPF project's .csproj file. The location of the .dll file will vary, but below is the example line:
```
<ItemGroup>
    <Analyzer Include="..\..\..\external\visual-studio-ui\external\standard-ui\src\Microsoft.StandardUI.SourceGenerator\bin\$(Configuration)\netstandard2.0\Microsoft.StandardUI.SourceGenerator.dll" />
</ItemGroup>
```

3. Add these line to project's `AssemblyInfo.cs` file. Be sure to replace the `FileName` with the correct name:
```C#
using Microsoft.StandardUI;
using Microsoft.VisualStudioUI;


[assembly: ImportStandardControl(typeof(IFileName))]
```

4. Once above steps are done, the UI control will be available for use in WPF. Below is example of how it can be used:

```xml
<Window x:Class="Sample.VSWin.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:namespace="clr-namespace:NameSpace"
        mc:Ignorable="d"
        Title="MainWindow" Height="1000" Width="1200">
    <StackPanel>
        <namespace:FileName/>
    </StackPanel>
</Window>
```