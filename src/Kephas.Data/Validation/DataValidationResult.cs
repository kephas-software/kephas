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
    using System.Diagnostics.Contracts;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;

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
        /// Initializes a new instance of the <see cref="DataValidationResult"/> class.
        /// </summary>
        public DataValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResult"/>
        /// class by adding the items to the result.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public DataValidationResult(params IDataValidationResultItem[] items)
        {
            Contract.Requires(items != null);

            this.Add(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationResult"/> class
        /// by adding a new item with the provided parameters.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">(Optional) name of the member.</param>
        /// <param name="severity">(Optional) the severity.</param>
        public DataValidationResult(string message, string memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
        {
            Requires.NotNull(message, nameof(message));

            this.Add(message, memberName, severity);
        }

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

        /// <summary>
        /// Adds the items to the validation result.
        /// </summary>
        /// <param name="items">The items to add.</param>
        /// <returns>
        /// This <see cref="DataValidationResult"/>.
        /// </returns>
        public DataValidationResult Add(params IDataValidationResultItem[] items)
        {
            Contract.Requires(items != null);

            this.items.AddRange(items);
            return this;
        }

        /// <summary>
        /// Adds a result item to the validation result.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="memberName">(Optional) name of the member.</param>
        /// <param name="severity">(Optional) the severity.</param>
        /// <returns>
        /// This <see cref="DataValidationResult"/>.
        /// </returns>
        public DataValidationResult Add(string message, string memberName = null, DataValidationSeverity severity = DataValidationSeverity.Error)
        {
            Requires.NotNull(message, nameof(message));

            this.items.Add(new DataValidationResultItem(message, memberName, severity));
            return this;
        }
    }
}