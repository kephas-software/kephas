// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAnnotationInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Information for constructing annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements.Construction
{
    /// <summary>
    /// Information for constructing annotations.
    /// </summary>
    public interface IAnnotationInfo : INamedElementInfo
    {
        /// <summary>
        /// Gets a value indicating whether multiple annotations of the same kind are allowed to be placed the same model element.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple annotations of the same kind are allowed; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If multiple annotations of the same kind are allowed, the qualified name will have a generated suffix 
        /// to allow the annotation to be unique within the members collection.
        /// </remarks>
        bool AllowMultiple { get; }
    }
}