# Umbraco Reusable Content Picker

This repo contains a content picker property for [Umbraco CMS](https://github.com/umbraco/umbraco-cms). It has been created specially to support a smooth editorial flow when working with reusable content from the Umbraco content tree, enabling the creation of new reusable content items while remaining in context of the page being edited. 

![Reusable Content Picker](/docs/img/reusable-content-picker.gif)

The Reusable Content Picker can be used as a drop-in replacement for the built-in Multinode Treepicker.

## Installation

Install the [NuGet package](https://www.nuget.org/packages/Our.Umbraco.ReusableContentPicker/) in your Umbraco project:

```
dotnet add MyProject package Our.Umbraco.ReusableContentPicker
```

## Quick start

1. Install the package.
2. Define a content type with the alias `itemFolder`, and allow it as root content.
3. Create a content node of type `itemFolder` in the root of the Umbraco content tree.
4. Define the content types that should be used for reusable content items, and allow them as children to the `itemFolder` content type.
   - _Note: Due to a [bug in Umbraco](https://github.com/umbraco/Umbraco-CMS/pull/12577), reusable content types cannot be language variant._
5. Add a Reusable Content Picker property to any content type in the Umbraco content tree.
6. Edit content and have fun!

## Configuration and defaults

The only configurable option for the Reusable Content Picker datatype is whether or not it should be possible to pick multiple content items. The remaining functionality is based either on runtime defaults that can be [altered programatically](docs/changing-the-runtime-defaults.md), or on configuration of other parts of the Umbraco backoffice.

### Root folder

The root folder is the starting point when the editors pick reusable content items. 

When looking for the root folder, Reusable Content Picker expects to find a content node with the content type alias `itemFolder` in the root of the Umbraco content tree. 

### Allowed content item types

When determining which types of reusable content items that can be created by the editors, Reusable Content Picker adheres to the configuration of the `itemFolder` content type. In other words, the editors can create reusable content items of the types allowed as children to the `itemFolder` content type.  

_Note: Due to a [bug in Umbraco](https://github.com/umbraco/Umbraco-CMS/pull/12577), reusable content types cannot be language variant._

### Item folders

If the editors are expected to create a lot of reusable content items, it might be prudent to let them structure the reusable content items in new item folders beneath the root folder.

This can be achieved by allowing the `itemFolder` content type to be a child of itself, provided that `itemFolder` lives up to two requirements:

1. It must not vary by culture.
2. It must not have any required properties.

### Troubleshooting your setup

If things aren't working after following the setup guide above, check the Umbraco log. Any configuration errors should be reflected in corresponding log errors.

## More topics

- [Fine tuning the editor experience](docs/fine-tuning-the-editor-experience.md) - a few tips & tricks for making the editor experience even better.
- [Changing the runtime defaults](docs/changing-the-runtime-defaults.md) - how to change the behavior of Reusable Content Picker to suit your specific needs.
