using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ReusableContentPicker.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using UmbracoConstants = Umbraco.Cms.Core.Constants;

namespace Our.Umbraco.ReusableContentPicker.PropertyEditors
{
    [DataEditor(
            "Our.Umbraco.ReusableContentPicker",
            "Reusable Content Picker",
            "~/App_Plugins/Our.Umbraco.ReusableContentPicker/views/editor.html",
            ValueType = ValueTypes.Text,
            Group =  UmbracoConstants.PropertyEditors.Groups.Pickers,
            Icon = "icon-page-add"
        )
    ]
    public class ReusableContentPickerPropertyEditor : DataEditor
    {
        private readonly IIOHelper _ioHelper;

        public ReusableContentPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory, IIOHelper ioHelper, EditorType type = EditorType.PropertyValue) 
            : base(dataValueEditorFactory, type)
        {
            _ioHelper = ioHelper;
        }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ReusableContentPickerConfigurationEditor(_ioHelper);

        protected override IDataValueEditor CreateValueEditor() => DataValueEditorFactory.Create<ReusableContentPickerPropertyValueEditor>(Attribute);

        public class ReusableContentPickerPropertyValueEditor : DataValueEditor, IDataValueReference
        {
            public ReusableContentPickerPropertyValueEditor(
                ILocalizedTextService localizedTextService,
                IShortStringHelper shortStringHelper,
                IJsonSerializer jsonSerializer,
                IIOHelper ioHelper,
                DataEditorAttribute attribute)
                : base(localizedTextService, shortStringHelper, jsonSerializer, ioHelper, attribute)
            {
            }

            public IEnumerable<UmbracoEntityReference> GetReferences(object value)
            {
                var asString = value == null ? string.Empty : value as string ?? value.ToString();
                var udis = asString.ParseContentUdis();
                return udis.Select(udi => new UmbracoEntityReference(udi));
            }
        }
    }
}
