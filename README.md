The Blazor Component Renderer is little utility designed to convert Blazor components into HTML strings dynamically. This tool enables developers to render basic Blazor UI components server-side, facilitating the generation of static content from dynamic components. 
It's particularly useful for creating email templates, generating content for static pages, or rendering dynamic web content for environments that do not support interactive sessions.

By integrating directly with the Blazor framework, this utility simplifies the process of transforming component output into well-formed HTML, making it a useful tool for developers looking to extend the capabilities of their web applications beyond traditional browser-based interactions. 
Whether you're looking to automate emails, generate PDFs from web pages, or pre-render pages for faster load times, the Blazor Component Renderer provides a robust, flexible, and efficient solution.


Key Features:

Seamless Integration: Works natively with Blazor Server applications.
High Performance: Optimized to handle complex component hierarchies with minimal overhead.
Flexibility: Supports extensive customization of component rendering to meet diverse application requirements.
Developer Friendly: Designed with a focus on ease of use, with comprehensive documentation and support for rapid integration.
This utility is ideal for organizations and developers looking to enhance their .NET applications with high-quality, programmatically generated HTML content derived from Blazor components.


I created a very small NuGet Package for this.

```csharp
dotnet add package BlazorComponentToString --version 1.0.0
```

Limitations:

Will not work with any components that use JSInterop.

I have also not tested any of this on WebAssembly. Please let me know if it works.


Big thanks to Andrew Lock and his Blog Post : https://andrewlock.net/exploring-the-dotnet-8-preview-rendering-blazor-components-to-a-string/


### Basic Usage


### You are going to need an ILogger and an ILoggerFactory as well as the language-ext package. https://github.com/louthy/language-ext

I use Serilog for logging, however you should be able to use whatever you like. You have the source to everything so feel free to do whatever you want.

Program.cs Configuration

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddBlazorComponentToStringRenderer();

        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext());
            
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        
        app.Run();
    }
}
```
### In a Class

```csharp
public class SomeClass
{
    private readonly IArcBlazorComponentRenderer _arcBlazorComponentRenderer;

    public SomeClass(IArcBlazorComponentRenderer arcBlazorComponentRenderer)
    {
        _arcBlazorComponentRenderer = arcBlazorComponentRenderer;
    }
}
```

In a Page Component

```csharp
@page "/"
@layout MainLayout
@inject IArcBlazorComponentRenderer ArcBlazorComponentRenderer

 <div><button @onclick="RenderComponentToString">Render Component To String</button></div>
 <div>
     @RenderedComponentValue
 </div>

@code
{
    private MarkupString RenderedComponentValue { get; set; } = new MarkupString();

    private async Task RenderComponentToString()
    {

        if (ArcBlazorComponentRenderer is null)
        {

            return;
        }
        
        // Assuming you have an EmailModel class and EmailTemplateComponent component
        var emailModel = new EmailModel()
            {
                From = "jimi@hendrixisgod.com",
                To = "thepope@thepope.com",
                Subject = "Jimi Hendrix is God",
                PlainTextBody = "Jimi Hendrix is God",
                HtmlBody = "<h1>Jimi Hendrix is God</h1>",
                SomeOtherNestedStuff = new SomeOtherNestedStuff()
                {
                    // Create a list of integers
                    SomeIntegers = [1, 2, 3, 4, 5],

                    // Create a list of strings
                    SomeStrings = ["one", "two", "three", "four", "five"],


                }
            };


        var cts = new CancellationTokenSource();

        var ct = cts.Token;

        var renderComponentToStringResults = await ArcBlazorComponentRenderer.RenderComponentToString<EmailModel, EmailTemplateComponent>(emailModel, ct);

        if (!renderComponentToStringResults.IsSuccess)
        {
            var exception = renderComponentToStringResults.HandleError(error => error);

            return;
        }

        RenderedComponentValue = new MarkupString(renderComponentToStringResults.HandleSuccess(success => success) ?? string.Empty);

    }
}

```



