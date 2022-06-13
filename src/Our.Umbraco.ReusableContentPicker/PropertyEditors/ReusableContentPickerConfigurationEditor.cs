using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Our.Umbraco.ReusableContentPicker.PropertyEditors;

public class ReusableContentPickerConfigurationEditor : ConfigurationEditor<ReusableContentPickerConfiguration>
{
    public ReusableContentPickerConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
    {
    }
}