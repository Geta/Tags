Tags
====

Geta Tags for EPiServer CMS

The latest version 0.5 is compiled for .NET 4 and EPiServer 7. It will use the legacy editor to render the property in edit mode. To add Tags as a new property to your page types you need to use the UIHint attribute like in the this example:
```csharp
[UIHint("Tags")]
public virtual string Tags { get; set; }
```

For an introduction see: [Tags for EPiServer CMS] (http://www.frederikvig.com/2011/07/tags-for-episerver-cms/) and [Tags version 0.2 released for EPiServer CMS] (http://www.frederikvig.com/2011/09/tags-version-0-2-released-for-episerver-cms/).