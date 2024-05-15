using BlazorComponentRenderer.Renderer.Interfaces;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace BlazorComponentRenderer.Renderer.Classes;

/// <summary>
/// Provides functionality to convert model properties to a format suitable for component parameters.
/// This class analyzes model properties and constructs a dictionary of component parameters,
/// handling type conversions and validation.
/// </summary>
public class ParameterConverter : IParameterConverter
{
    private readonly ILogger<ParameterConverter> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterConverter"/> class.
    /// </summary>
    /// <param name="logger">The logger used to log warnings or errors during the conversion process.</param>
    public ParameterConverter(ILogger<ParameterConverter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Converts properties of a model to <see cref="ParameterView"/> suitable for initializing a Blazor component.
    /// </summary>
    /// <typeparam name="TModel">The type of the model containing the data.</typeparam>
    /// <typeparam name="TComponent">The component type that will receive the parameters.</typeparam>
    /// <param name="model">The model instance containing the data to convert to component parameters.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the converted parameters or an error if the conversion fails.</returns>
    public Task<Result<ParameterView>> ConvertToParameters<TModel, TComponent>(TModel model) where TComponent : IComponent
    {
        try
        {
            var componentType = typeof(TComponent);
            var parameters = new Dictionary<string, object>();

            foreach (var propInfo in componentType.GetProperties())
            {
                if (!Attribute.IsDefined(propInfo, typeof(ParameterAttribute))) continue;

                var propName = propInfo.Name;
                var modelProperty = typeof(TModel).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                var valueToAdd = modelProperty switch
                {
                    // If modelProperty is null and the property type of the component is assignable from the model type, use the entire model.
                    null when propInfo.PropertyType.IsAssignableFrom(typeof(TModel)) => model,

                    // If modelProperty is null, log a warning and return null.
                    null => LogAndReturnNull(propName, typeof(TModel), propInfo.PropertyType),

                    // Otherwise, use the ConvertProperty method to convert and assign the value.
                    _ => ConvertProperty(modelProperty, model, propInfo)
                };

                if (valueToAdd != null)
                {
                    parameters[propName] = valueToAdd;
                }
            }
            return Task.FromResult(new Result<ParameterView>(ParameterView.FromDictionary(parameters)));
        }
        catch (System.Exception ex)
        {
            return Task.FromResult(new Result<ParameterView>(ex));
        }
    }

    /// <summary>
    /// Logs a warning when a property is expected but not found or is of a different type than expected.
    /// </summary>
    /// <param name="propertyName">The name of the property being checked.</param>
    /// <param name="expectedType">The type that was expected for the property.</param>
    /// <param name="actualType">The actual type found for the property.</param>
    /// <returns>Always returns null, indicating that no suitable value could be assigned to the property.</returns>
    private object? LogAndReturnNull(string propertyName, Type expectedType, Type actualType)
    {
        _logger.LogWarning("Type mismatch for property {PropertyName}: Expected {ExpectedType}, got {ActualType}",
            propertyName, expectedType, actualType);
        return null;
    }

    /// <summary>
    /// Converts and assigns the value of a model property to a corresponding component property if possible.
    /// </summary>
    /// <param name="modelProperty">The property info of the model property being converted.</param>
    /// <param name="model">The model instance containing the data.</param>
    /// <param name="propInfo">The property info of the component property to assign to.</param>
    /// <returns>The converted property value or null if the conversion was not possible or the property was null.</returns>
    private object? ConvertProperty(PropertyInfo modelProperty, object? model, PropertyInfo propInfo)
    {
        var modelValue = modelProperty.GetValue(model);
        if (modelValue == null)
        {
            // If the model value is null and the property can accept nulls, assign it directly
            return propInfo.PropertyType.IsClass || Nullable.GetUnderlyingType(propInfo.PropertyType) != null ? modelValue : LogAndWarnTypeMismatch(modelProperty, propInfo);
        }

        // If the types are assignable, return the model value directly
        if (propInfo.PropertyType.IsAssignableFrom(modelProperty.PropertyType))
        {
            return modelValue;
        }

        // If the target property is expecting a type that can be converted from the model property's type
        try
        {
            var convertedValue = Convert.ChangeType(modelValue, propInfo.PropertyType);
            return convertedValue;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Failed to convert property value from type {ModelType} to {ComponentType}", modelProperty.PropertyType, propInfo.PropertyType);
        }

        return LogAndWarnTypeMismatch(modelProperty, propInfo);
    }

    /// <summary>
    /// Logs a warning when a type mismatch occurs during the property conversion process.
    /// </summary>
    /// <param name="modelProperty">The model property that was being converted.</param>
    /// <param name="propInfo">The component property that was expected to receive the value.</param>
    /// <returns>Always returns null, signaling a failure in the conversion process due to type mismatch.</returns>
    private object? LogAndWarnTypeMismatch(PropertyInfo modelProperty, PropertyInfo propInfo)
    {
        _logger.LogWarning("Type mismatch for property {PropertyName}: Expected {ExpectedType}, got {ActualType}",
            propInfo.Name, propInfo.PropertyType, modelProperty.PropertyType);
        return null;
    }
}
