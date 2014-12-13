// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementConstructorInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Structure providing information about constructing elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Factory
{
    using System.Reflection;

    /// <summary>
    /// Structure providing information about constructing elements.
    /// </summary>
    public class ElementConstructorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementConstructorInfo" /> class.
        /// </summary>
        /// <param name="modelSpace">The model space.</param>
        /// <param name="nativeElement">The native element.</param>
        public ElementConstructorInfo(IModelSpace modelSpace, MemberInfo nativeElement)
        {
            this.ModelSpace = modelSpace;
            this.NativeElement = nativeElement;
            this.NativeTypeInfo = nativeElement as TypeInfo;
        }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public IModelSpace ModelSpace { get; private set; }

        /// <summary>
        /// Gets the native element.
        /// </summary>
        /// <value>
        /// The native element.
        /// </value>
        public MemberInfo NativeElement { get; private set; }

        /// <summary>
        /// Gets the native type information.
        /// </summary>
        /// <value>
        /// The native type information.
        /// </value>
        public TypeInfo NativeTypeInfo { get; private set; }
    }
}