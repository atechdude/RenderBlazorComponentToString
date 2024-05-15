using LanguageExt.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentRenderer.Renderer.Interfaces;

public interface IParameterConverter
{
    Task<Result<ParameterView>> ConvertToParameters<TModel, TComponent>(TModel model) where TComponent : IComponent;
}