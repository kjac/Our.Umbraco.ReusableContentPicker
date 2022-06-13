using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.ReusableContentPicker.PropertyEditors;

public class ReusableContentPickerConfiguration
{
    [ConfigurationField("multiSelect", "Multi select", "boolean", Description = "Allow selecting multiple items")]
    public bool MultiSelect { get; set; }
}