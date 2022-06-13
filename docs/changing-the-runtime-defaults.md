# Changing the runtime defaults

In some cases, the defaults that Reusable Content Picker is based upon won't work for your requirements:

- If you don't like the `itemFolder` alias.
- If you want to use different content types for the root and item folders.
- If you need multiple root folders, e.g. for multi-site setups.
- ...

Fortunately you can change the runtime defaults programatically.

This is done by registering your own implementation of [`IFolderService`](/src/Our.Umbraco.ReusableContentPicker/Infrastructure/IFolderService.cs). The [default implementation](/src/Our.Umbraco.ReusableContentPicker/Infrastructure/FolderService.cs) has been created to be as extensible as possible, but of course you can also implement your own from scratch. 

The interface contains its own documentation in code. For the sake of keeping it up to date, it won't be repeated here, so go have a look at [`IFolderService`](/src/Our.Umbraco.ReusableContentPicker/Infrastructure/IFolderService.cs) instead. 

The following sample `IFolderService` implementation extends the default implementation to:

- Use custom content types for item and root folders (different ones for each).
- Enable reusable content to be defined at site level, rather than at content root level.

```cs
// sample folder service that:
// - customizes the content type alias for all item folders
// - lets us have a different content type for root folders
// - enables site specific reusable content items 
public class MyFolderService : FolderService
{
    private readonly IContentTypeService _contentTypeService;
    private readonly ISqlContext _sqlContext;

    public MyFolderService(IContentService contentService, IContentTypeService contentTypeService, ISqlContext sqlContext)
        : base(contentService)
    {
        _contentTypeService = contentTypeService;
        _sqlContext = sqlContext;
    }

    // customizes the item folder content type alias. we'll always use the same item folder type,
    // no matter where the edited content (contentId) is located in the content tree
    public override string GetItemFolderContentTypeAlias(int contentId)
        => "customItemFolder";

    // enables site specific reusable content items by nesting the root folders at site level
    // instead of at content tree root level, and customizes the root folder content type
    public override IContent GetRootFolder(int contentId)
    {
        // get the content currently being edited
        var content = ContentService.GetById(contentId);

        // grab the ID of the content ancestor at root level (the site)
        var siteId = content.GetAncestorIds().FirstOrDefault();
        if (siteId <= 0)
        {
            // no ancestor at root level, we're currently editing the site itself
            siteId = contentId;
        }

        // get the content type of the root folder (for querying below)
        var rootFolderContentType = _contentTypeService.Get("customRootFolder");
        if (rootFolderContentType == null)
        {
            return null;
        }

        // construct a query that looks for content of the custom root folder type
        var rootFolderQuery = _sqlContext.Query<IContent>().Where(c => c.ContentTypeId == rootFolderContentType.Id);

        // return the first custom root folder available under the site (if any)
        var rootFolder = ContentService
            .GetPagedChildren(siteId, 0, 1, out _, rootFolderQuery)
            .FirstOrDefault();
        return rootFolder;
    }
}
```
