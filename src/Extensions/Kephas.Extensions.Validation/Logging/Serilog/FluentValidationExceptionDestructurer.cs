using FluentValidation.Results;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;
using FluentValidationException = FluentValidation.ValidationException;

namespace Kephas.Extensions.Logging.Serilog;

/// <summary>
/// Destructurer for <see cref="FluentValidation.ValidationException"/>.
/// </summary>
public class FluentValidationExceptionDestructurer : ExceptionDestructurer
{
    /// <inheritdoc cref="P:Serilog.Exceptions.Destructurers.IExceptionDestructurer.TargetTypes" />
    public override Type[] TargetTypes =>
    [
        typeof(FluentValidationException)
    ];

    /// <inheritdoc cref="M:Serilog.Exceptions.Destructurers.IExceptionDestructurer.Destructure(System.Exception,Serilog.Exceptions.Core.IExceptionPropertiesBag,System.Func{System.Exception,System.Collections.Generic.IReadOnlyDictionary{System.String,System.Object}})" />
    public override void Destructure(
        Exception exception,
        IExceptionPropertiesBag propertiesBag,
        Func<Exception, IReadOnlyDictionary<string, object?>?> destructureException)
    {
        base.Destructure(exception, propertiesBag, destructureException);

#pragma warning disable CA1062 // Validate arguments of public methods
        var validationException = (FluentValidationException)exception;
        propertiesBag.AddProperty(
            nameof(FluentValidationException.Errors),
            GetErrors(validationException.Errors));
#pragma warning restore CA1062 // Validate arguments of public methods
    }

    internal static IEnumerable<object> GetErrors(IEnumerable<ValidationFailure> errors) =>
        errors.Select(e => new
        {
            e.PropertyName,
            AttemptedValue = e.AttemptedValue?.ToString(),
            e.ErrorMessage,
            ErrorMessageArgs =
                e.FormattedMessagePlaceholderValues?.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString()),
            e.ErrorCode,
            e.Severity
        });
}