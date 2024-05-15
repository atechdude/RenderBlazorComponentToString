using Microsoft.AspNetCore.Components;

namespace BlazorComponentRenderer.Renderer.Interfaces;

public interface IComponentRenderer
{
    Task<string> RenderAsync<TComponent>(ParameterView parameters, CancellationToken cancellationToken) where TComponent : IComponent;
}