using Umbraco.Cms.Core.Models;

namespace Our.Umbraco.ReusableContentPicker.Infrastructure;

public interface IFolderService
{
    /// <summary>
    /// Gets the content type alias for item folders applicable when editing a specific piece of content in the content tree
    /// </summary>
    /// <param name="contentId">The ID of the content being edited (the content that contains the Reusable Content Picker)</param>
    /// <returns>The item folder content type alias</returns>
    string GetItemFolderContentTypeAlias(int contentId);

    /// <summary>
    /// Gets the root folder applicable when editing a specific piece of content in the content tree
    /// </summary>
    /// <param name="contentId">The ID of the content being edited (the content that contains the Reusable Content Picker)</param>
    /// <returns>The root folder</returns>
    IContent GetRootFolder(int contentId);

    /// <summary>
    /// Gets a specific item folder 
    /// </summary>
    /// <param name="contentId">The ID of the content being edited (the content that contains the Reusable Content Picker)</param>
    /// <param name="folderId">The ID of the item folder to get</param>
    /// <returns>The item folder</returns>
    IContent GetItemFolder(int contentId, int folderId);

    /// <summary>
    /// Creates an item folder
    /// </summary>
    /// <param name="name">The name of the item folder (as specified by the editors)</param>
    /// <param name="parentFolder">The parent folder to create the item folder beneath</param>
    /// <param name="itemFolderContentTypeAlias">The item folder content type alias returned from <see cref="GetItemFolderContentTypeAlias"/></param>
    /// <returns>The new item folder</returns>
    IContent CreateItemFolder(string name, IContent parentFolder, string itemFolderContentTypeAlias);
}