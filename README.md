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


Limitations:
Most liekly will not work with any components that use JSInterop.


Big thanks to Andrew Lock and his Blog Post : https://andrewlock.net/exploring-the-dotnet-8-preview-rendering-blazor-components-to-a-string/


### Basic Usage


I use Serilog, Doesn't everyone these days? If you don't have Serilog setup then please do so. You are going to need an ILogger and an ILoggerFactory.

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
```csharp

## Injecting the IArcBlazorComponentRenderer

public class SomeClass
{
    private readonly IArcBlazorComponentRenderer _arcBlazorComponentRenderer;

    public SomeClass(IArcBlazorComponentRenderer arcBlazorComponentRenderer)
    {
        _arcBlazorComponentRenderer = arcBlazorComponentRenderer;
    }
}
```

