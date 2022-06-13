using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.ReusableContentPicker.Infrastructure;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

namespace Our.Umbraco.ReusableContentPicker.Api;

[PluginController("ReusableContent")]
public class EditorApiController : UmbracoAuthorizedJsonController
{
    private readonly IContentService _contentService;
    private readonly IContentTypeService _contentTypeService;
    private readonly ISqlContext _sqlContext;
    private readonly IUmbracoMapper _umbracoMapper;
    private readonly ILocalizationService _localizationService;
    private readonly IFolderService _folderService;

    public EditorApiController(IContentService contentService, IContentTypeService contentTypeService, ISqlContext sqlContext, IUmbracoMapper umbracoMapper, ILocalizationService localizationService, IFolderService folderService)
    {
        _contentService = contentService;
        _contentTypeService = contentTypeService;
        _sqlContext = sqlContext;
        _umbracoMapper = umbracoMapper;
        _localizationService = localizationService;
        _folderService = folderService;
    }

    [HttpGet]
    public ActionResult LoadFolder(int contentId, int? folderId, int pageNumber, string query = null, string culture = null)
    {
        culture ??= _localizationService.GetDefaultLanguageIsoCode();
        var itemFolderContentTypeAlias = _folderService.GetItemFolderContentTypeAlias(contentId);

        var itemFolderContentType = _contentTypeService.Get(itemFolderContentTypeAlias);
        if (itemFolderContentType == null)
        {
            return NotFound($"Could not find item folder content type: {itemFolderContentTypeAlias}");
        }

        var folder = folderId.HasValue
            ? _folderService.GetItemFolder(contentId, folderId.Value)
            : _folderService.GetRootFolder(contentId);

        if (folder == null)
        {
            return NotFound($"Could not find folder in content tree: {folderId?.ToString() ?? "(root folder)"}");
        }

        var folderQuery = _sqlContext.Query<IContent>().Where(c => c.ContentTypeId == itemFolderContentType.Id);
        var itemsQuery = _sqlContext.Query<IContent>().Where(c => c.ContentTypeId != itemFolderContentType.Id);
        if (query.IsNullOrWhiteSpace() == false)
        {
            itemsQuery = itemsQuery.Where(c => c.Name.Contains(query));
        }

        var folders = _contentService.GetPagedChildren(folder.Id, 0, 100, out _, folderQuery, Ordering.By("name"));

        const int pageSize = 20;
        var items = _contentService.GetPagedChildren(folder.Id, pageNumber - 1, pageSize, out var totalItems, itemsQuery, Ordering.By("name"));

        return Ok(new
        {
            current = Map(folder, culture),
            folders = MapAll(folders, culture),
            items = MapAll(items, culture),
            pageNumber = pageNumber,
            totalPages = (int)Math.Round((double)totalItems / pageSize, MidpointRounding.ToPositiveInfinity),
            folderContentTypeAlias = itemFolderContentTypeAlias
        });
    }

    [HttpGet]
    public ActionResult GetItem(int id, string culture = null)
    {
        culture ??= _localizationService.GetDefaultLanguageIsoCode();

        var item = _contentService.GetById(id);
        if (item == null)
        {
            return NotFound($"Could not find item: {id}");
        }

        var itemFolderContentTypeAlias = _folderService.GetItemFolderContentTypeAlias(id);

        var parent = _contentService.GetParent(id);
        if (parent == null || parent.ContentType.Alias != itemFolderContentTypeAlias)
        {
            return NotFound($"Could not validate item: {id}");
        }

        return Ok(new
        {
            item = Map(item, culture)
        });
    }

    [HttpPost]
    public ActionResult CreateFolder(CreateFolderRequest request)
    {
        var parent = _contentService.GetById(request.ParentId);
        if (parent == null)
        {
            return BadRequest($"No content found for parent ID: {request.ParentId}");
        }

        var itemFolderContentTypeAlias = _folderService.GetItemFolderContentTypeAlias(request.ParentId);

        var parentContentType = _contentTypeService.Get(parent.ContentTypeId);
        if (parentContentType.AllowedContentTypes.Any(c => c.Alias == itemFolderContentTypeAlias) == false)
        {
            return BadRequest($"Content type: {parentContentType.Alias} for parent: {request.ParentId} does not allow sub content of type: {itemFolderContentTypeAlias}");
        }

        var folder = _folderService.CreateItemFolder(request.Name, parent, itemFolderContentTypeAlias);
        if (folder == null)
        {
            return BadRequest($"Could not create a new folder: {request.Name} of type: {itemFolderContentTypeAlias} under parent: ({parent.Id})");
        }

        var culture = request.Culture ?? _localizationService.GetDefaultLanguageIsoCode();

        return Ok(new
        {
            folder = Map(folder, culture)
        });
    }

    private ContentItemBasic<ContentPropertyBasic> Map(IContent content, string culture)
        => _umbracoMapper.Map<IContent, ContentItemBasic<ContentPropertyBasic>>(content, context => context.SetCulture(culture));

    private IEnumerable<ContentItemBasic<ContentPropertyBasic>> MapAll(IEnumerable<IContent> content, string culture)
        => content.Select(c => Map(c, culture));
    public class CreateFolderRequest
    {
        public int ParentId { get; set; }

        public string Name { get; set; }

        public string Culture { get; set; }
    }
}