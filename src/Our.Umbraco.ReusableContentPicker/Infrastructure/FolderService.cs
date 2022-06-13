using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.ReusableContentPicker.Infrastructure;

public class FolderService : IFolderService
{
    public FolderService(IContentService contentService)
    {
        ContentService = contentService;
    }

    protected IContentService ContentService { get; }

    public virtual string GetItemFolderContentTypeAlias(int contentId) => "itemFolder";

    public virtual IContent GetRootFolder(int contentId)
        => ContentService.GetRootContent().FirstOrDefault(c => c.ContentType.Alias == GetItemFolderContentTypeAlias(contentId));

    public virtual IContent GetItemFolder(int contentId, int folderId) => ContentService.GetById(folderId);

    public virtual IContent CreateItemFolder(string name, IContent parentFolder, string itemFolderContentTypeAlias)
    {
        var folder = ContentService.Create(name, parentFolder, itemFolderContentTypeAlias);
        ContentService.SaveAndPublish(folder);
        return folder;
    }
}