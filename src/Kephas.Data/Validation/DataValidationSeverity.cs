// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValidationSeverity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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