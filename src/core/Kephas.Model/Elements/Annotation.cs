// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Annotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Definition class for annotations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Elements
{
    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Definition class for annotations.
    /// </summary>
    public class Annotation : NamedElementBase<IAnnotation, IAnnotationInfo>, IAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Annotation"/> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <param name="modelSpace"> The model space.</param>
        public Annotation(IAnnotationInfo elementInfo, IModelSpace modelSpace)
            : base(elementInfo, modelSpace)
        {
            this.AllowMultiple = elementInfo.AllowMultiple;
        }

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
        public bool AllowMultiple { get; }
    }
}