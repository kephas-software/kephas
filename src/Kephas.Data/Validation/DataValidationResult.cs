// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationResult.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data validation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Encapsulates the result of a data validation.
    /// </summary>
    public class DataValidationResult : IDataValidationResult
    {
        /// <summary>
        /// The validation result indicating that the validation succeeded without any issues.
        /// </summary>
        public static readonly DataValidationResult Success = new DataValidationResult();

        /// <summary>
        /// The items.
        /// </summary>
        private readonly IList<IDataValidationResultItem> items = new List<IDataValidationResultItem>();

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<IDataValidationResultItem> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }
}