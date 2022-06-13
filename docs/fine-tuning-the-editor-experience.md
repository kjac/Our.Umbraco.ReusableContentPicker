# Fine tuning the editor experience

Reusable Content Picker relies heavily on the built-in editor experience of Umbraco for content editing. This means that the editors by default will have access to the full editor experience when creating and editing reusable content items.

This might not be the best solution for your editors.

Technically, the reusable content items are fully-fledged content nodes in the Umbraco content tree. However - the editors probably shouldn't think of them as individually routable pages, but rather as small bits of content. Thus it might make sense to remove the "Info" content app when editing reusable content items. 

It would probably also make sense to remove the preview option for the reusable content items, as they likely won't support previewing.

Provided you can identify reusable content items at runtime (i.e. by their content types), you can achieve these things by implementing a [notification handler](https://our.umbraco.com/documentation/Fundamentals/Code/Subscribing-To-Notifications/) for [`SendingContentNotification`](https://our.umbraco.com/Documentation/Reference/Notifications/EditorModel-Notifications/):

```cs
// sample SendingContentNotification handler that limits the available 
// content apps and disables preview for reusable content items
public class EditorSendingContentNotificationHandler : INotificationHandler<SendingContentNotification>
{
    public void Handle(SendingContentNotification notification)
    {
        // is this a piece of reusable content (type alias ends with "Reusable")?
        if (notification.Content.ContentTypeAlias.EndsWith("Reusable") == false)
        {
            return;
        }

        // find the "content" content app
        var umbContent = notification.Content.ContentApps.FirstOrDefault(c => c.Alias == "umbContent");
        if (umbContent == null)
        {
            // this ShouldNotHappen(tm)
            return;
        }

        // only show the "content" content app in the editor
        notification.Content.ContentApps = new[] { umbContent };

        // remove the preview option from the editor
        notification.Content.AllowPreview = false;
    }
}

// composer for registering the SendingContentNotification handler
public class SiteComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddNotificationHandler<SendingContentNotification, EditorSendingContentNotificationHandler>();
    }
}
```

_Note: The content rollback option is placed on the "Info" content app, so removing the "Info" content app will efficiently make the editors unable to perform rollback. This is a trade-off you'll have to consider._