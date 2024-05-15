using BlazorComponentRenderer.Renderer.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorComponentRenderer.Renderer.Classes;

/// <summary>
/// Handles rendering of Blazor components into HTML strings.
/// This class is responsible for creating and managing the lifecycle of Blazor components,
/// executing them within a dedicated service scope, and converting their output to HTML.
/// </summary>
public class ComponentRenderer : IComponentRenderer
{
    private readonly IServiceProvider _services;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentRenderer"/> class.
    /// </summary>
    /// <param name="services">The service provider used to create new service scopes and resolve dependencies.</param>
    /// <param name="loggerFactory">The factory used to create loggers for logging information and errors.</param>
    public ComponentRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
    {
        _services = services;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Asynchronously renders a specified Blazor component into an HTML string using the provided parameters.
    /// </summary>
    /// <typeparam name="TComponent">The type of the Blazor component to render. This type must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="parameters">The parameters to pass to the component for rendering.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the render operation before completion.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the rendered HTML string.</returns>
    /// <exception cref="System.Exception">Throws if there is an error during the component's rendering process.</exception>
    public async Task<string> RenderAsync<TComponent>(ParameterView parameters, CancellationToken cancellationToken) where TComponent : IComponent
    {
        using var scope = _services.CreateScope();
        var htmlRenderer = new HtmlRenderer(scope.ServiceProvider, _loggerFactory);
        try
        {
            var component = Activator.CreateInstance<TComponent>();
            // Use the Dispatcher to handle the render on the correct thread
            var renderResult = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                // Render the component and convert the result to an HTML string
                var result = await htmlRenderer.RenderComponentAsync<TComponent>(parameters);
                return result.ToHtmlString();
            }).ConfigureAwait(false);
            return renderResult;
        }
        catch (System.Exception ex)
        {
            var logger = _loggerFactory.CreateLogger<ComponentRenderer>();
            logger.LogError(ex, "Error rendering component {ComponentName}", typeof(TComponent).Name);
            throw;
        }
    }
}