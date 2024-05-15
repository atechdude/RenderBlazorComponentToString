using BlazorComponentRenderer.Renderer.Interfaces;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using BlazorComponentRenderer.Renderer.Extensions;

namespace BlazorComponentRenderer.Renderer.Classes;

/// <summary>
/// Facilitates the rendering of Blazor components into HTML strings by coordinating with a component renderer and a parameter converter.
/// This class serves as a higher-level abstraction for converting model data into parameters and then rendering components based on those parameters.
/// </summary>
public class ArcBlazorComponentRenderer : IArcBlazorComponentRenderer
{
    private readonly IComponentRenderer _componentRenderer;
    private readonly IParameterConverter _parameterConverter;
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArcBlazorComponentRenderer"/> class.
    /// </summary>
    /// <param name="componentRenderer">The service responsible for rendering Blazor components.</param>
    /// <param name="parameterConverter">The service responsible for converting model data into component parameters.</param>
    /// <param name="loggerFactory">The factory used to create loggers for logging information and errors.</param>
    public ArcBlazorComponentRenderer(IComponentRenderer componentRenderer, IParameterConverter parameterConverter, ILoggerFactory loggerFactory)
    {
        _componentRenderer = componentRenderer;
        _parameterConverter = parameterConverter;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Asynchronously renders a Blazor component into an HTML string using model data.
    /// </summary>
    /// <typeparam name="TModel">The type of the model containing the data.</typeparam>
    /// <typeparam name="TComponent">The Blazor component type to render, which must implement <see cref="IComponent"/>.</typeparam>
    /// <param name="model">The model instance containing the data to be passed to the component.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the render operation before completion.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <see cref="Result{String}"/> containing either the rendered HTML string or an error.</returns>
    public async Task<Result<string>> RenderComponentToString<TModel, TComponent>(TModel model, 
        CancellationToken cancellationToken = default) where TComponent : IComponent
    {
        var convertModelParametersResults = await
            _parameterConverter.ConvertToParameters<TModel, TComponent>(
                model!);
        if (!convertModelParametersResults.IsSuccess)
        {
            return new Result<string>(convertModelParametersResults.HandleError(error => error));
        }

        var renderComponentResult = convertModelParametersResults.HandleSuccess(result => result);

        return await _componentRenderer.RenderAsync<TComponent>(renderComponentResult,cancellationToken);
    }
}