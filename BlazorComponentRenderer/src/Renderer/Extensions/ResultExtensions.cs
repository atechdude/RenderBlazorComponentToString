using BlazorComponentRenderer.Renderer.Enums;
using BlazorComponentRenderer.Renderer.Exception;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;

namespace BlazorComponentRenderer.Renderer.Extensions;

/// <summary>
///     Provides extension methods to simplify handling results from operations encapsulated within Result objects.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    ///     Handles errors by executing the provided error handler action and returning the caught exception.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The Result object to handle.</param>
    /// <param name="errorHandler">The action to execute when an error occurs.</param>
    /// <returns>The caught exception, if an error occurred; otherwise, null.</returns>
    public static System.Exception? HandleError<T>(
        this Result<T> result,
        Action<System.Exception> errorHandler)
    {
        System.Exception? exception = null;

        var match = result.Match(
            success => success,
            error =>
            {
                exception = error;
                errorHandler(error);
                return default!;
            });

        return exception;
    }

    /// <summary>
    ///     Handles errors by executing the provided error handler function and returning the error result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <typeparam name="E">The type of the error result.</typeparam>
    /// <param name="result">The Result object to handle.</param>
    /// <param name="errorHandler">The function to execute when an error occurs.</param>
    /// <returns>The error result, if an error occurred; otherwise, the default value of type E.</returns>
    public static E? HandleError<T, E>(
        this Result<T> result,
        Func<System.Exception, E> errorHandler)
    {
        E? errorResult = default;

        var match = result.Match(
            success => success,
            error =>
            {
                errorResult = errorHandler(error);
                return default!;
            });

        return errorResult;
    }

    /// <summary>
    ///     Handles successful results by executing the provided success handler action and returning the result value.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The Result object to handle.</param>
    /// <param name="successHandler">The action to execute when the operation is successful.</param>
    /// <returns>The result value, if the operation was successful; otherwise, null.</returns>
    public static T? HandleSuccess<T>(
        this Result<T> result,
        Action<T> successHandler)
    {
        T? value = default;

        var match = result.Match<T>(
            success =>
            {
                value = success;
                successHandler(success);
                return success;
            },
            error => default!);

        return value;
    }

    /// <summary>
    ///     Handles successful results by executing the provided success handler function and returning the modified result
    ///     value.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The Result object to handle.</param>
    /// <param name="successHandler">The function to execute when the operation is successful.</param>
    /// <returns>The modified result value, if the operation was successful; otherwise, null.</returns>
    public static T? HandleSuccess<T>(
        this Result<T> result,
        Func<T, T> successHandler)
    {
        T? value = default;

        var match = result.Match(
            success =>
            {
                value = successHandler(success);
                return value;
            },
            error => default!);

        return value;
    }

    /// <summary>
    ///     Handles errors for a Result object, allowing both logging and custom error-handling actions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The Result object to handle.</param>
    /// <param name="errorHandler">The action to execute when an error occurs.</param>
    /// <param name="logAction">The logging action to execute, taking an ILogger and the exception as parameters.</param>
    /// <param name="logger">The ILogger instance to be used for logging.</param>
    /// <returns>Returns the caught exception if an error occurred; otherwise, returns null.</returns>
    public static System.Exception? HandleErrorWithLogging<T>(
        this Result<T> result,
        Action<System.Exception> errorHandler,
        Action<ILogger, System.Exception> logAction,
        ILogger logger)
    {
        System.Exception? exception = null;

        var match = result.Match(
            success => success,
            error =>
            {
                exception = error;
                logAction(logger, exception); // Log before handling the error
                errorHandler(exception);
                return default!;
            });

        return exception;
    }

    /// <summary>
    ///     Handles successful results for a Result object, allowing both logging and custom success-handling actions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="result">The Result object to handle.</param>
    /// <param name="successHandler">The action to execute when the operation is successful.</param>
    /// <param name="logAction">The logging action to execute, taking an ILogger and the success value as parameters.</param>
    /// <param name="logger">The ILogger instance to be used for logging.</param>
    /// <returns>Returns the value of the result if the operation was successful; otherwise, returns null.</returns>
    public static T? HandleSuccessWithLogging<T>(
        this Result<T> result,
        Action<T> successHandler,
        Action<ILogger, T> logAction,
        ILogger logger)
    {
        T? value = default;

        var match = result.Match(
            success =>
            {
                value = success;
                logAction(logger, value); // Log before handling the success
                successHandler(value);
                return value;
            },
            error => default!);

        return value;
    }

    public static async Task<Result<T>> ProcessOperationAsync<T>(
        this Task<Result<T>> operationTask,
        string operationName,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var result = await operationTask;

            // Handle failure scenario
            if (!result.IsSuccess)
            {
                logger.LogError("Failure in {OperationName}: {Error}", operationName, result.HandleError(ex=>ex)?.Message);
                return result; // This already contains the error details.
            }

            // Check for null result which should be considered a failure in this context
            if (result.HandleSuccess(success => success) != null)
            {
                return result;
            }
            var nullException = new AppException(ExceptionType.EmptyResult, $"{operationName} returned null result.");
            logger.LogError(nullException, "Null result in {OperationName}", operationName);
            return new Result<T>(nullException);

            // If everything is fine, return the successful result.
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Exception in {OperationName}", operationName);
            return new Result<T>(ex);
        }
    }
}