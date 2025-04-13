using Kephas.Validation;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;

namespace Kephas.Extensions.Logging.Serilog;

/// <summary>
/// Destructurer for <see cref="ValidationException"/>.
/// </summary>
public class ValidationExceptionDestructurer : ExceptionDestructurer
{
    /// <inheritdoc cref="P:Serilog.Exceptions.Destructurers.IExceptionDestructurer.TargetTypes" />
    public override Type[] TargetTypes =>
    [
        typeof(ValidationException)
    ];    

    /// <inheritdoc cref="M:Serilog.Exceptions.Destructurers.IExceptionDestructurer.Destructure(System.Exception,Serilog.Exceptions.Core.IExceptionPropertiesBag,System.Func{System.Exception,System.Collections.Generic.IReadOnlyDictionary{System.String,System.Object}})" />
    public override void Destructure(
        Exception exception,
        IExceptionPropertiesBag propertiesBag,
        Func<Exception, IReadOnlyDictionary<string, object?>?> destructureException)
    {
        base.Destructure(exception, propertiesBag, destructureException);

#pragma warning disable CA1062 // Validate arguments of public methods
        var validationException = (ValidationException)exception;
        var validationResult = validationException.ValidationResult;
        if(validationResult is null)
        {
            return;
        }

        propertiesBag.AddProperty(
            nameof(ValidationException.ValidationResult),
            new
            {
                validationResult.IsValid,
                Errors = FluentValidationExceptionDestructurer.GetErrors(validationResult.Errors),
            });
#pragma warning restore CA1062 // Validate arguments of public methods
    }    
}