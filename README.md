Geta Tags for EPiServer
====
## What is Geta Tags?

Geta Tags is library that adds tagging functionality to EPiServer content.

## How to get started?

Start by installing NuGet package (use [EPiServer NuGet](http://nuget.episerver.com/)):

    Install-Package Geta.Tags

The latest version is compiled for .NET 4.5 and EPiServer 9. 
Geta Tags library uses [tag-it](https://github.com/aehlke/tag-it) jQuery UI plugin for selecting tags.
To add Tags as a new property to your page types you need to use the UIHint attribute like in this example:

```csharp
[UIHint("Tags")]
public virtual string Tags { get; set; }

[TagsGroupKey("mykey")]
[UIHint("Tags")]
public virtual string Tags { get; set; }

[CultureSpecific]
[UIHint("Tags")]
public virtual string Tags { get; set; }
```

Use ITagEngine to query for data:
```csharp
IEnumerable<ContentData> GetContentByTag(string tagName);
IEnumerable<ContentData> GetContentsByTag(Tag tag);
IEnumerable<ContentData> GetContentsByTag(string tagName, ContentReference rootContentReference);
IEnumerable<ContentData> GetContentsByTag(Tag tag, ContentReference rootContentReference);
IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames);
IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags);
IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames, ContentReference rootContentReference);
IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags, ContentReference rootContentReference);
```
