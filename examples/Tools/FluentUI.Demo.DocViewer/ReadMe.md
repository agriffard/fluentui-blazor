# FluentUI.Demo.DocViewer

It's a simple library that transforms all markdown documentation files (.md)
into equivalent HTML pages.

## Get Started

1. Before to use this library, you need to register the **services** in the `Program.cs` file.
   The specified assembly will be used to search for the markdown pages and razor components.

   ```csharp
   builder.Services.AddHttpClient();

   builder.Services.AddSingleton<DocViewerService>(factory =>
   {
       return new DocViewerService()
       {
           PageTitle = "{0} - FluentUI Blazor Components",
           ComponentsAssembly = typeof(Client._Imports).Assembly,
           ResourcesAssembly = typeof(Client._Imports).Assembly
       };
   });
   ```

2. You need to embed all markdown files in the project, adding this configuration in the `.csproj` file.

   ```xml
   <ItemGroup>
     <EmbeddedResource Include="**/*.md" />
   </ItemGroup>
   ```

3. You need to capture all request to display the documentation pages.
   You can use the following `DocViewer.razor` file to capture all requests.

   ```razor
   @page "/{*Path:nonfile}"
   @using FluentUI.Demo.DocViewer.Components
   
   <MarkdownViewer Route="@Path" />
   
   @code
   {
       [Parameter]
       public string Path { get; set; } = string.Empty;
   }
   ```

## Header: Title and Routing

Each markdown file contains a header that defines the page title and the
the route to this documentation page.

```markdown
---
title: Button
route: /Button
---
```

## Code section

You can include code snippets in your markdown file.
The code will be highlighted using the specified language: `razor`, `html`, `js`, ....

You need to add associated the JavaScript and CSS files to the `index.html` file.

**Example**
```html
<!-- Include highlight.js -->
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.10.0/styles/vs.min.css" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.10.0/highlight.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.10.0/languages/csharp.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/highlightjs-cshtml-razor@2.1.1/dist/cshtml-razor.min.js"></script>
```

## Razor component

You can include razor components in your markdown file using `{{ MyCounter }}` syntax.
The library will search the file `MyCounter.razor` in the registered assembly and render it.

```markdown
{{ MyCounter }}
```

## Example

```markdown
---
title: Button
route: /Button
---

# My Counter Page

'''razor
@page "/MyCounter"

Value: @Value

<button @onclick="Button_Click">Click</button>

@code {
    private int Value = 0;

    private void Button_Click()
    {
        Value++;
    }
}
'''

## Demo

{{ MyCounter }}

This code is a live demo of a Counter

```