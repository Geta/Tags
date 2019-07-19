# Changelog

All notable changes to this project will be documented in this file.

## [4.0.11]

### Fixed

- Fix #83: Miss icons on Search, Cancel button
- Fix #84: Geta Tags displays incorrect after deletion
- Fix #85: Can view search data on other page
- Fix #86: Total tag should show number of displaying tags

## [4.0.10]

### Fixed

- Fix Geta Tags Management admin page, see issue #79

## [4.0.9]

### Fixed

- Fix getting tags property when editing tag in admin page

## [4.0.8]

### Changed

- Install package as protected module. When installing this version make sure you remove the `Geta.Tags` folder from `/modules. The new one is created under ~/modules/protected/

## [4.0.7]

- Upgrading System.Security.Cryptography.Xml package

## [4.0.6]

- Fix for missing references in TagEngine

## [4.0.5]

- Fix for removing a tag from a page doesn't remove the page from the list where it's displayed ([#70](https://github.com/Geta/Tags/issues/70))

## [4.0.4]

- Fixed a maintenance job to not remove tags from the localized content.

## [4.0.3]

- Fixed UIHint attribute to support multiple.

## [4.0.2]

- Setting language for the tag for all values so that tags are removed properly.

## [4.0.1]

- Fixed a bug when tag changes in one language, remove tags in other languages.

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
