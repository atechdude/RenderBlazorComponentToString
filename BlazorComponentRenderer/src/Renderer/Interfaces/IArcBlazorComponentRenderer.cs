using LanguageExt.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentRenderer.Renderer.Interfaces;

public interface IArcBlazorComponentRenderer
{
    Task<Result<string>> RenderComponentToString<TModel, TComponent>(TModel model,
        CancellationToken cancellationToken = default)
        where TComponent : IComponent;
}