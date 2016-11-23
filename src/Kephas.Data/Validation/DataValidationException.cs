// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data validation exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
    using System;
    using System.Linq;

    using Kephas.Data.Resources;

    /// <summary>
    /// Base class for validation exceptions.
    /// </summary>
    public class DataValidationException : DataException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class.
        /// </summary>
        /// <param name="validatedEntity">The validated entity.</param>
        /// <param name="validationResult">The validation result.</param>
        public DataValidationException(object validatedEntity, IDataValidationResult validationResult)
          : base(string.Format(Strings.DataValidationException_Message, validatedEntity, string.Join(Environment.NewLine, validationResult.Select(e => e.Message))))
        {
            this.ValidatedEntity = validatedEntity;
            this.ValidationResult = validationResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected DataValidationException(string message)
          : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        protected DataValidationException(string message, Exception inner)
          : base(message, inner)
        {
        }

        /// <summary>
        /// Gets the validated entity.
        /// </summary>
        /// <value>
        /// The validated entity.
        /// </value>
        public object ValidatedEntity { get; }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        /// <value>
        /// The validation result.
        /// </value>
        public IDataValidationResult ValidationResult { get; }
    }
}