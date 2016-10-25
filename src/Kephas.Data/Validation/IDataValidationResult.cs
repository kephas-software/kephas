// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValidationResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataValidationResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Values that represent validation severities.
    /// </summary>
    public enum DataValidationSeverity
    {
        /// <summary>
        /// The validation resulted in an error.
        /// </summary>
        Error,

        /// <summary>
        /// The validation resulted in a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// The validation resulted in an information.
        /// </summary>
        Info,
    }

    /// <summary>
    /// Interface for validation result.
    /// </summary>
    public interface IDataValidationResult : IEnumerable<IDataValidationResultItem>
    {
    }

    /// <summary>
    /// Interface for validation result item.
    /// </summary>
    public interface IDataValidationResultItem
    {
        /// <summary>
        /// Gets the validation result severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        DataValidationSeverity Severity { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDataValidationResult"/>
    /// </summary>
    public static class ValidationResultExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the validation result has errors.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns>
        /// <c>true</c> if the validation result contains errors, <c>false</c> if not.
        /// </returns>
        public static bool HasErrors(this IDataValidationResult result)
        {
            Contract.Requires(result != null);

            return result.Any(i => i.Severity == DataValidationSeverity.Error);
        }

        /// <summary>
        /// Gets only the errors items from this validation result.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns>
        /// An enumration of errors.
        /// </returns>
        public static IEnumerable<IDataValidationResultItem> Errors(this IDataValidationResult result)
        {
            Contract.Requires(result != null);

            return result.Where(i => i.Severity == DataValidationSeverity.Error);
        }
    }
}