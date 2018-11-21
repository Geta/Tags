# Changelog

All notable changes to this project will be documented in this file.

## [4.0.0]

- Removed obsolete methods which used old page API.

Removed from `ITagEngine`:

```csharp
PageDataCollection GetPagesByTag(string tagName);
PageDataCollection GetPagesByTag(Tag tag);
PageDataCollection GetPagesByTag(string tagName, PageReference rootPageReference);
PageDataCollection GetPagesByTag(Tag tag, PageReference rootPageReference);
IEnumerable<PageReference> GetPageReferencesByTags(string tagNames);
IEnumerable<PageReference> GetPageReferencesByTags(IEnumerable<Tag> tags);
IEnumerable<PageReference> GetPageReferencesByTags(string tagNames, PageReference rootPageReference);
IEnumerable<PageReference> GetPageReferencesByTags(IEnumerable<Tag> tags, PageReference rootPageReference);
```

Removed from `ITagRepository`:

```csharp
IEnumerable<Tag> GetTagsByPage(Guid pageGuid);
```

Removed from `ITagService`:

```csharp
IEnumerable<Tag> GetTagByPage(Guid pageGuid);
```

- Added a method to get content by content Guid to the `ITagService` - `IEnumerable<Tag> GetTagsByContent(Guid contentGuid);`
- Changed all methods which return IEnumerable, to always return a result instead of null.


## [3.0.4]
 - Fixed #65 set groupkey based on the current content language instead of CMS language


## [3.0.3]
 - Added #59 Add sortable tags

## [3.0.2]
 - Fixed #57 Add PagedList to modules/Geta.Tags/ClientResources/styles

## [3.0.1]

### Added
- Added Changelog
- Added checkbox on the edit tag page to also update the tags defined in the content properties

