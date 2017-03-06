// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedFullName.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the qualified full name class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// A qualified full name.
    /// </summary>
    internal class QualifiedFullName
    {
        /// <summary>
        /// The type definition seperator.
        /// </summary>
        private const char TypeDefinitionSeperator = ',';

        /// <summary>
        /// Zero-based index of the type name.
        /// </summary>
        private const int TypeNameIndex = 0;

        /// <summary>
        /// Zero-based index of the assembly name.
        /// </summary>
        private const int AssemblyNameIndex = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedFullName"/> class.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        public QualifiedFullName(string typeDefinition)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeDefinition));

            var parts = typeDefinition.Split(TypeDefinitionSeperator);
            this.TypeName = parts[TypeNameIndex].Trim();
            var assemblyName = (parts.Length > AssemblyNameIndex) ? parts[AssemblyNameIndex].Trim() : null;
            this.AssemblyName = string.IsNullOrWhiteSpace(assemblyName) ? null : new AssemblyName(assemblyName);
        }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public AssemblyName AssemblyName { get; set; }
    }
}