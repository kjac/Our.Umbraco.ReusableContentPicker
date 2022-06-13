using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.ReusableContentPicker.Extensions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace Our.Umbraco.ReusableContentPicker.PropertyEditors;

public class ReusableContentPickerValueConverter : PropertyValueConverterBase
{
    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

    public ReusableContentPickerValueConverter(IPublishedSnapshotAccessor publishedSnapshotAccessor)
    {
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) => 
        propertyType.EditorAlias.Equals("Our.Umbraco.ReusableContentPicker");

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        => PropertyCacheLevel.Snapshot;

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
        => IsSingleNodePicker(propertyType)
            ? typeof(IPublishedContent)
            : typeof(IEnumerable<IPublishedContent>);

    public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) 
        => source?.ToString()?.ParseContentUdis();

    public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel cacheLevel, object source, bool preview)
    {
        if (source == null)
        {
            return null;
        }

        var udis = (Udi[])source;
        var isSingleNodePicker = IsSingleNodePicker(propertyType);

        var multiNodeTreePicker = new List<IPublishedContent>();

        var publishedSnapshot = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot();
        foreach (var udi in udis)
        {
            var content = publishedSnapshot.Content.GetById(udi);
            if (content == null)
            {
                continue;
            }
            multiNodeTreePicker.Add(content);
            if (isSingleNodePicker)
            {
                break;
            }
        }

        if (isSingleNodePicker)
        {
            return multiNodeTreePicker.FirstOrDefault();
        }
        return multiNodeTreePicker;
    }
    
    // for future expansion
    private static bool IsSingleNodePicker(IPublishedPropertyType propertyType) => false;
}