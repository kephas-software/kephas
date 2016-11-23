// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationSeverity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data validation severity class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Validation
{
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
}