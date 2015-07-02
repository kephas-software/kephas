// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Metadata keys used to tie programming model entities into their back-end hosting implementations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Internals
{
    /// <summary>
    /// Metadata keys used to tie programming model entities into their back-end hosting implementations.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The sharing boundary implemented by an import.
        /// </summary>
        public const string SharingBoundaryImportMetadataConstraintName = "SharingBoundaryNames";

        /// <summary>
        /// Marks an import as "many".
        /// </summary>
        public const string ImportManyImportMetadataConstraintName = "IsImportMany";
    }
}