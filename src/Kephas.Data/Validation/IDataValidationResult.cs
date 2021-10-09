// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValidationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataValidationResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;

    /// <summary>
    /// Interface for validation result.
    /// </summary>
    public interface IDataValidationResult : IOperationResult, IEnumerable<IDataValidationResultItem>
    {
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
            result = result ?? throw new ArgumentNullException(nameof(result));

            return result.Any(i => i.Severity == DataValidationSeverity.Error);
        }

        /// <summary>
        /// Gets only the errors items from this validation result.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <returns>
        /// An enumeration of errors.
        /// </returns>
        public static IEnumerable<IDataValidationResultItem> GetErrors(this IDataValidationResult result)
        {
            result = result ?? throw new ArgumentNullException(nameof(result));

            return result.Where(i => i.Severity == DataValidationSeverity.Error);
        }
    }
}