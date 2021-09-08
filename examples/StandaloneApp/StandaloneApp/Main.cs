using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudioUI.Options;

using Microsoft.VisualStudioUI.Options.Models;

namespace Microsoft.VisualStudioUI.StandaloneApp
{
    public static class Main
    {
        public static OptionCards CreateOptionCards()
        {
            OptionCards cards = new OptionCards();
            cards.AddCard(TestEnableCard());
            cards.AddCard(ITunesArtworkCard());
            cards.AddCard(SigningCard());
            cards.AddCard(SiriCard());
            cards.AddCard(ICloudCard());
            cards.AddCard(WalletCard());
            cards.AddCard(BackgroundModesCard());
            cards.AddCard(MyCard());
            cards.AddCard(SecondCard());

            return cards;
        }

        private static OptionCard TestEnableCard()
        {
            OptionCard card = new OptionCard()
            {
                Label = "Test enable"
            };

            var variablesTable = new KeyValueTableEntryOption(new ViewModelProperty<ImmutableArray<KeyValueItem>>("")) {
                AddButtonTitle = "Add",
                RemoveButtonTitle = "Remove",
                ValuesColumnTitle = "Values",
                VariablesColumnTitle = "Variables"
            };
            card.AddOption(variablesTable);

            var dependOn = new CheckBoxOption(BoolProp(false)) { ButtonLabel = "enable" };

            var warning = new ViewModelProperty<Message?>("", new Message("warning", MessageSeverity.Warning));
            warning.Bind();
            var error = new ViewModelProperty<Message?>("", new Message("warning", MessageSeverity.Error));

            card.AddOption(dependOn);
            card.AddOption(new SeparatorOption());
            card.AddOption(new CheckBoxOption(BoolProp(false)) { ButtonLabel = "test", DisablebilityDependsOn = dependOn });
            card.AddOption(new DocButtonOption(StringProp("test"), "test") { DisablebilityDependsOn = dependOn });
            card.AddOption(new TextOption(StringProp("")) { Label = "test warning", ValidationMessage = warning, DisablebilityDependsOn = dependOn });
            card.AddOption(new TextOption(StringProp("")) { Label = "test error", ValidationMessage = error, DisablebilityDependsOn = dependOn });
            card.AddOption(new DirectoryOption(StringProp("")) { Label = "Choose Firectory", Hint = "hint", DisablebilityDependsOn = dependOn });
            card.AddOption(new ProjectFileOption(StringProp("")) { Label = "Choose File", Hint = "hint" , DisablebilityDependsOn = dependOn });
            TextOption fileEntry = new TextOption(StringProp(""))
            {
                Label = "Uncompressed resource extensions",
                DisablebilityDependsOn = dependOn
            };
            fileEntry.MacroMenuItems = ImmutableArray.CreateRange(
                new[] { new MacroMenuItem("Project Directory", "$(ProjectDir)"),
                new MacroMenuItem("Solution Directory", "$(SolutionDir)"),
                });
            card.AddOption(fileEntry);
            card.AddOption(new StepperOption(new ViewModelProperty<int>("", 1000000)) { Label = "Port", DisablebilityDependsOn = dependOn, Hint = "test",Maximum = 10000000,Minimum = 0 });
            card.AddOption(new SeparatorOption());

            var disable = new CheckBoxOption(BoolProp(false)) { ButtonLabel = "Disable", Enable = BoolProp(true) };
            var disableBtn = new ButtonOption() { Label = "Test Disable", Name = "test"};
            disableBtn.Clicked += (sender, e)=> {
                disable.Enable.Value = !disable.Enable.Value;
            };
            card.AddOption(disableBtn);
            card.AddOption(disable);

            return card;
        }

        private static OptionCard ITunesArtworkCard()
        {
            var card = new OptionCard() { Label = "iTunes Artwork" };
            List<ScaledImageFile> imagelist = new List<ScaledImageFile>();
            imagelist.Add(new ScaledImageFile(512, 512, "1X") { Path = null }); ;
            imagelist.Add(new ScaledImageFile(1024, 1024, "2X") { Path = "/Users/vstester/Projects/iOS/iOS/Assets.xcassets/AppIcon.appiconset/Icon1024.png " });
            ViewModelProperty<ImmutableArray<ScaledImageFile>> imageArray = new ViewModelProperty<ImmutableArray<ScaledImageFile>>("", imagelist.ToImmutableArray());
            
            var image = new ScaledImageFileOption(imageArray, "Unset");
            image.ImageArray.Bind();
           
            image.UnsetImage += (sender, e) =>
            {
                int a = 0;
                a += 1;
            };

            card.AddOption(image);

            var signInLabel = new LabelOption()
            {
                Label = ("Apple ID"),
                Name = "Sign in and select a team to enable Automatic Provisioning",
                IsBold = true,
                AllowSpaceForLabel = false,
                ValidationMessage = new ViewModelProperty<Message>("", new Message(null, MessageSeverity.Warning))
            };
            card.AddOption(signInLabel);
            return card;
        }

        private static OptionCard SigningCard()
        {
            var card = new OptionCard() { Label = "Signing" };

            ViewModelProperty<bool> automaticSigningEnabled = BoolProp(false);
            ViewModelProperty<bool> manualSigningEnabled = BoolProp(!automaticSigningEnabled.Value);

            var signingRadioGroup = new RadioButtonGroup();
            var manual = new RadioButtonOption(signingRadioGroup, manualSigningEnabled)
            {
                Label = "Scheme",
                ButtonLabel = "Manual Provisioning",
                Description = "Set provisioningSet provisioningSet provisioningSet provisioningSet provisioningSet provisioningSet provisioningSet provisioning"
            };

            var auto = new RadioButtonOption(signingRadioGroup, automaticSigningEnabled)
            {
                ButtonLabel = "Automatic Provisioning",
                Description = "Set provisioningSet provisioningSet provisioningSet provisioningSet provisioningSet provisioningSet provisioningSet provisioning"
            };

            card.AddOption(manual);
            card.AddOption(auto);

            var manualSigningOption1 = new CheckBoxOption(BoolProp(false))
            {
                ButtonLabel = "Some option for manual signing",
                VisibilityDependsOn = manual
            };
            card.AddOption(manualSigningOption1);


            var autoSigningOption1 = new CheckBoxOption(BoolProp(false))
            {
                ButtonLabel = "Some option for automatic signing",
                VisibilityDependsOn = auto
            };
            var autoSigningOption2 = new CheckBoxOption(BoolProp(false))
            {
                ButtonLabel = "Another option for automatic signing",
                VisibilityDependsOn = auto
            };
            card.AddOption(autoSigningOption1);
            card.AddOption(autoSigningOption2);

            var btn1 = new ButtonOption() { Name = "Hide" };
            var btn2 = new ButtonOption() { Name = "2"};
            var btn3 = new ButtonOption() { Name = "show" };
            
            card.AddOption(btn1);
            card.AddOption(btn2);
            card.AddOption(btn3);

            LabelOption btn = new LabelOption();
            btn.Name = "Test";
            btn.IsBold = false;
            ProgressIndicatorOption pro = new ProgressIndicatorOption(btn);
            pro.ShowSpinner.Bind();
            pro.Label = "ProgresssIndicator";
            card.AddOption(pro);

            return card;
        }

        private static OptionCard SiriCard()
        {
            var card = new OptionCard();
            card.AddOption(
                new SwitchOption(BoolProp(true))
                {
                    ButtonLabel = "Siri",
                    Description = "Allows your application to handle Siri requests.",
                    Hint = "Hint: Allows your application to handle Siri requests.",
                }
            );

            card.AddOption(new StepperOption(new ViewModelProperty<int>("", 100))
            {
                Label = "Port",
                Hint = "hint"
            });

            return card;
        }

        private static OptionCard ICloudCard()
        {
            var card = new OptionCard();
            var switchableOption = new SwitchOption(BoolProp(true))
            {
                ButtonLabel = "iCloud",
                Description = "Allows your application to store data in the cloud and lets users share their data across devices.Allows your application to store data in the cloud and lets users share their data across devices.Allows your application to store data in the cloud and lets users share their data across devices.Allows your application to store data in the cloud and lets users share their data across devices.",
                Hint = "Hint: Allows your application to store data in the cloud and lets users share their data across devices.",
            };

            card.AddOption(switchableOption);

            ImmutableArray<string> list = ImmutableArray.Create("test1", "test2", "test3");

            var keychainAccessGroupsList = new StringListOption(new ViewModelProperty<ImmutableArray<string>>("t", list))
            {
                AddToolTip = "Click to add an Keychain Access Group",
                RemoveToolTip = "Click to remove the selected Keychain Access Group",
                Label = "Keychain Groups",
                DefaultValue = "BundleIdentifier",
                PrefixValue = "AppIdentifierPrefix"
            };

            card.AddOption(keychainAccessGroupsList);

            return card;
        }

        private static OptionCard WalletCard()
        {
            var card = new OptionCard();
            var walletSwitchableOption = new SwitchOption(BoolProp(true))
            {
                ButtonLabel = "Wallet",
                Description = "Allows your application to manage passes, tickets, gift cards, and loyalty cards. It supports a variety of bar code formats."
            };
            var passTypeRadioGroup = new RadioButtonGroup();
            var alltype = new RadioButtonOption(passTypeRadioGroup, BoolProp(false))
            {
                Label = "Pass Types",
                ButtonLabel = "Allow all team pass types",
                VisibilityDependsOn = walletSwitchableOption
            };

            var subtype = new RadioButtonOption(passTypeRadioGroup, BoolProp(false))
            {
                ButtonLabel = "Allow subset of pass types",
                VisibilityDependsOn = walletSwitchableOption
            };

            ImmutableArray<CheckBoxlistItem> CheckBoxList = ImmutableArray.Create(
                new CheckBoxlistItem("Pass types1", false),
                new CheckBoxlistItem("Pass types2", false),
                new CheckBoxlistItem("Pass types3", false)
            );

            bool allowReording = true;
            var testCheckBoxList = new CheckBoxListOption(new ViewModelProperty<ImmutableArray<CheckBoxlistItem>>("", CheckBoxList), allowReording) { VisibilityDependsOn = walletSwitchableOption };
            card.AddOption(walletSwitchableOption);
            card.AddOption(alltype);
            card.AddOption(subtype);
            card.AddOption(testCheckBoxList);
            

            return card;
        }

        private static OptionCard BackgroundModesCard()
        {
            var card = new OptionCard();
            var backgroundModesSwitch = new SwitchOption(BoolProp(true))
            {
                ButtonLabel = "Background Modes",
                Description = "Set allowed background modes.",
                Hint = "This is the hint"
            };
            card.AddOption(backgroundModesSwitch);

            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Audio, AirPlay and Picture in Picture"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Location updates"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Voice over IP"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "External accessory communication"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Uses Bluetooth LE accessories"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Acts as Bluetooth LE accessory"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Background fetch"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Remote notifications"));
            card.AddOption(BackgroundModeCheckBox(backgroundModesSwitch, "Background processing"));

            return card;
        }

        private static CheckBoxOption BackgroundModeCheckBox(SwitchOption backgroundModeSwitch, string label)
        {
            return new CheckBoxOption(BoolProp(false))
            {
                ButtonLabel = label,
                VisibilityDependsOn = backgroundModeSwitch
            };
        }

        private static OptionCard MyCard()
        {
            var card = new OptionCard()
            {
                Label = "My Card"
            };

            card.AddOption(
                new TextOption(StringProp("text1"))
                {
                    Label = "Text 1",
                    Hint = "This is the hint to show!"
                }
            );
            card.AddOption(
                new TextOption(StringProp("text2"))
                {
                    Label = "Text 2"
                }
            );
            card.AddOption(new SeparatorOption());
            card.AddOption(
                new TextOption(StringProp("text3"))
                {
                    Label = "Text"
                }
            );
            card.AddOption(
                new ComboBoxOption<string>(StringProp("option1"), StringArrayProp(new[] { "option1", "option2", "option3" }))
                {
                    Label = "Choices",
                    Hint = "This is the hint for the combo box"
                }
            );
            card.AddOption(
                new EditableComboBoxOption(StringProp("option1"), StringArrayProp(new[] { "option1", "option2", "option3" }))
                {
                    Label = "Editable choices",
                    Hint = "This is the hint for the editable combo box"
                }
            );

            TextOption fileEntry = new TextOption(StringProp(""))
            {
                Label = "fileEntry",
            };
            fileEntry.MacroMenuItems = ImmutableArray.CreateRange(
                new[] { new MacroMenuItem("Project Directory", "$(ProjectDir)"),
                new MacroMenuItem("Solution Directory", "$(SolutionDir)"),
                });
            card.AddOption(fileEntry);

            ViewModelProperty<ImmutableArray<string>> propertyItems = new ViewModelProperty<ImmutableArray<string>>("");
            var subMenuCombox = new ComboBoxOption<string>(StringProp("option1"), propertyItems)
            {
                Label = "Sub Menu",
                HasMultipleLevelMenu = true,
                Hint = "This is the hint for the editable combo box"
            };

            List<string> dropList = new List<string>();
            dropList.Add(subMenuCombox.CreateHeaderMenu("Areest"));
            dropList.Add("option1");
            dropList.Add("option2");
            dropList.Add(subMenuCombox.CreateSeperator());
            dropList.Add("None");
            propertyItems.Value = ImmutableArray.CreateRange(dropList).ToImmutableArray();

            card.AddOption(subMenuCombox);

            return card;
        }

        private static OptionCard SecondCard()
        {
            var card = new OptionCard()
            {
                Label = "Second Card"
            };
            card.AddOption(new TextOption(StringProp("text1")));
            card.AddOption(new TextOption(StringProp("text2")));
            card.AddOption(new TextOption(StringProp("text3")));
            card.AddOption(new TextOption(StringProp("text4")));
            card.AddOption(new TextOption(StringProp("text4")));
            card.AddOption(
                new DocButtonOption(StringProp("https://www.microsoft.com"), "Launch Browser for Doc")
                {
                    Label = "Learn more"
                }
            );

            return card;
        }

        public static ViewModelProperty<bool> BoolProp(bool defaultValue)
        {
            var prop = new ViewModelProperty<bool>("boolProp", defaultValue);
            prop.Bind();
            return prop;
        }

        public static ViewModelProperty<string> StringProp(string defaultValue) =>
            new ViewModelProperty<string>("stringProp", defaultValue);
        public static ViewModelProperty<ImmutableArray<string>> StringArrayProp(string[] defaultValue) =>
            new ViewModelProperty<ImmutableArray<string>>("stringArrayProp", ImmutableArray.Create(defaultValue));

        public static ViewModelProperty<bool[]> BoolArrayProp(bool[] defaultValue) =>
           new ViewModelProperty<bool[]>("boolArrayProp", defaultValue);

        public static ViewModelProperty<ImmutableArray<string>> ListProp(ImmutableArray<string> defaultValue) =>
            new ViewModelProperty<ImmutableArray<string>>("listProp", defaultValue);
    }
}
