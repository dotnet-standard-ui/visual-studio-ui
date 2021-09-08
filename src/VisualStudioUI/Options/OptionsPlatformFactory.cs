using System;

namespace Microsoft.VisualStudioUI.Options
{
    public abstract class OptionFactoryPlatform
    {
        private static OptionFactoryPlatform? _instance;

        public static void Initialize(OptionFactoryPlatform factory)
        {
            _instance = factory;
        }

        public static bool IsInitialized => _instance != null;

        public static OptionFactoryPlatform Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("OptionsPlatformFactory needs to be initialized");
                return _instance;
            }
        }

        public abstract OptionCardsPlatform CreateOptionCardsPlatform(OptionCards optionCards);
        public abstract OptionCardPlatform CreateOptionCardPlatform(OptionCard optionCard);

        public abstract OptionPlatform CreateTextOptionPlatform(TextOption textOption);
        public abstract OptionPlatform CreateComboBoxOptionPlatform<TItem>(ComboBoxOption<TItem> comboBoxOption)
            where TItem : class;
        public abstract OptionPlatform CreateEditableComboBoxOptionPlatform(EditableComboBoxOption editableComboBoxOption);
        public abstract OptionPlatform CreateDocButtonOptionPlatform(DocButtonOption docButtonOption);
        public abstract OptionPlatform CreateSeparatorOptionPlatform(SeparatorOption separatorOption);
        public abstract OptionPlatform CreateSwitchOptionPlatform(SwitchOption switchOption);
        public abstract OptionPlatform CreateStringListOptionPlatform(StringListOption stringListOption);
        public abstract OptionPlatform CreateImageViewOptionPlatform(ScaledImageFileOption imageFileOption);
        public abstract OptionPlatform CreateButtonOptionPlatform(ButtonOption buttonOption);

        public abstract OptionPlatform CreateCheckBoxOptionPlatform(CheckBoxOption checkBoxOption);
        public abstract OptionPlatform CreateRadioButtonOptionPlatform(RadioButtonOption radioButtonOption);

        public abstract OptionPlatform CreateCheckBoxListOptionPlatform(CheckBoxListOption checkBoxListOption);
        public abstract OptionPlatform CreateLabelOptionPlatform(LabelOption labelOption);
        public abstract OptionPlatform CreateStepperOptionPlatform(StepperOption stepperOption);
        public abstract OptionPlatform CreateProgressIndicatorOptionPlatform(ProgressIndicatorOption progressIndicatorOption);
        public abstract OptionPlatform CreateCreateDirectoryOptionlatform(DirectoryOption directoryOption);
        public abstract OptionPlatform CreateCreateProjectFileOptionlatform(ProjectFileOption projectFileOption);
        public abstract OptionPlatform CreateKeyValueTableEntryOptionPlatform(KeyValueTableEntryOption keyValueTableEntryOption);
    }
}
