using BlazorComponentRenderer.Renderer.Enums;

namespace BlazorComponentRenderer.Renderer.Exception;

/// <summary>
///     Application-specific exception class to handle various exception types.
/// </summary>
public class AppException : System.Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppException" /> class with a custom message.
    /// </summary>
    /// <param name="type">The type of exception.</param>
    /// <param name="message">The custom message.</param>
    public AppException(ExceptionType type, string message) : base(message)
    {
        Type = type;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppException" /> class with a custom message and an inner exception.
    /// </summary>
    /// <param name="type">The type of exception.</param>
    /// <param name="message">The custom message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AppException(ExceptionType type, string message, System.Exception? innerException) : base(message,
        innerException)
    {
        Type = type;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppException" /> class with a custom message and additional details.
    /// </summary>
    /// <param name="type">The type of exception.</param>
    /// <param name="message">The custom message.</param>
    /// <param name="details">Additional details about the exception.</param>
    public AppException(ExceptionType type, string message, string? details) : this(type, message)
    {
        Details = details;
    }

    /// <summary>
    ///     Gets the type of the exception.
    /// </summary>
    public ExceptionType Type { get; }

    /// <summary>
    ///     Gets or sets additional details about the exception.
    /// </summary>
    public string? Details { get; set; }
}