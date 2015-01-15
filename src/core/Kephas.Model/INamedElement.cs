// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedElement.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for named elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Model.Elements.Construction;

    /// <summary>
    /// Contract for named elements.
    /// </summary>
    [ContractClass(typeof(NamedElementContractClass))]
    public interface INamedElement
    {
        /// <summary>
        /// Gets the friendly name of the element.
        /// </summary>
        /// <value>
        /// The element name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the qualified name of the element.
        /// </summary>
        /// <value>
        /// The qualified name of the element.
        /// </value>
        /// <remarks>
        /// The qualified name is unique within the container's members. 
        /// Some elements have the qualified name the same as their name, 
        /// but others will use a discriminator prefix to avoid name collisions.
        /// For example, annotations use the "@" discriminator, dimensions use "^", and projections use ":".
        /// </remarks>
        string QualifiedName { get; }

        /// <summary>
        /// Gets the fully qualified name, starting from the root model space.
        /// </summary>
        /// <value>
        /// The fully qualified name.
        /// </value>
        /// <remarks>
        /// The fully qualified name is built up of qualified names separated by "/".
        /// </remarks>
        /// <example>
        /// <para>
        /// /: identifies the root model space.
        /// </para>
        /// <para>
        /// /^AppLayer: identifies the AppLayer dimension. 
        /// </para>
        /// <para>
        /// /:Primitives:Kephas:Core:Main:Global/String: identifies the String classifier within the :Primitives:Kephas:Core:Main:Global projection. 
        /// </para>
        /// <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name: identifies the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection. 
        /// </para>
        /// <para>
        /// /:MyModel:MyCompany:Contacts:Main:Domain/Contact/Name/@Required: identifies the Required attribute of the Name member of the Contact classifier within the :MyModel:MyCompany:Contacts:Main:Domain projection. 
        /// </para>
        /// </example>
        string FullQualifiedName { get; }

        /// <summary>
        /// Gets the container element.
        /// </summary>
        /// <value>
        /// The container element.
        /// </value>
        IModelElement Container { get; }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        IModelSpace ModelSpace { get; }

        /// <summary>
        /// Gets the element infos which constructed this element.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        IEnumerable<INamedElementInfo> UnderlyingElementInfos { get; }
    }

    /// <summary>
    /// Contract class for <see cref="INamedElement"/>.
    /// </summary>
    [ContractClassFor(typeof(INamedElement))]
    internal abstract class NamedElementContractClass : INamedElement
    {
        /// <summary>
        /// Gets the name of the model element.
        /// </summary>
        /// <value>
        /// The model element name.
        /// </value>
        public string Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return Contract.Result<string>();
            }
        }

        /// <summary>
        /// Gets the qualified name of the element.
        /// </summary>
        /// <value>
        /// The qualified name of the element.
        /// </value>
        /// <remarks>
        /// The qualified name is unique within the container's members.
        /// </remarks>
        public string QualifiedName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return Contract.Result<string>();
            }
        }

        /// <summary>
        /// Gets the fully qualified name, starting from the root model space.
        /// </summary>
        /// <value>
        /// The fully qualified name.
        /// </value>
        public string FullQualifiedName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return Contract.Result<string>();
            }
        }

        /// <summary>
        /// Gets the container element.
        /// </summary>
        /// <value>
        /// The container element.
        /// </value>
        public IModelElement Container
        {
            get
            {
                return Contract.Result<IModelElement>();
            }
        }

        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <value>
        /// The model space.
        /// </value>
        public IModelSpace ModelSpace
        {
            get
            {
                Contract.Ensures(Contract.Result<IModelSpace>() != null);
                return Contract.Result<IModelSpace>();
            }
        }

        /// <summary>
        /// Gets the element infos which constructed this element.
        /// </summary>
        /// <value>
        /// The element infos.
        /// </value>
        public IEnumerable<INamedElementInfo> UnderlyingElementInfos
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<INamedElementInfo>>() != null);
                return Contract.Result<IEnumerable<INamedElementInfo>>();
            }
        }
    }
}