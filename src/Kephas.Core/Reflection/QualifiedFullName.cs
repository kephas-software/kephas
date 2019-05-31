// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualifiedFullName.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the qualified full name class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A qualified full name.
    /// </summary>
    internal class QualifiedFullName
    {
        /// <summary>
        /// The type definition separator.
        /// </summary>
        private const char TypeDefinitionSeparator = ',';

        /// <summary>
        /// The generic closing bracket.
        /// </summary>
        private const char GenericClosingBracket = ']';

        /// <summary>
        /// Zero-based index of the assembly name.
        /// </summary>
        private const int AssemblyNameIndex = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedFullName"/> class.
        /// </summary>
        /// <param name="qualifiedFullName">The qualified full name.</param>
        public QualifiedFullName(string qualifiedFullName)
        {
            Requires.NotNullOrEmpty(qualifiedFullName, nameof(qualifiedFullName));

            var endGenericTypeNameIndex = qualifiedFullName.LastIndexOf(GenericClosingBracket);
            int typeNameLength;
            if (endGenericTypeNameIndex > 0)
            {
                typeNameLength = endGenericTypeNameIndex + 1;
            }
            else
            {
                var firstSeparatorIndex = qualifiedFullName.IndexOf(TypeDefinitionSeparator);
                typeNameLength = firstSeparatorIndex > 0 ? firstSeparatorIndex : qualifiedFullName.Length;
            }

            if (typeNameLength == qualifiedFullName.Length)
            {
                this.TypeName = qualifiedFullName;
                return;
            }

            this.TypeName = qualifiedFullName.Substring(0, typeNameLength);

            // the trailing parts contain the assembly name, version, and public key token
            var trailingParts = qualifiedFullName.Substring(typeNameLength)
                .Split(TypeDefinitionSeparator);

            if (trailingParts.Length <= AssemblyNameIndex)
            {
                return;
            }

            this.AssemblyName = this.GetAssemblyName(trailingParts[AssemblyNameIndex]);
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

        private AssemblyName GetAssemblyName(string rawAssemblyName)
        {
            var assemblyName = rawAssemblyName?.Trim();
            return string.IsNullOrWhiteSpace(assemblyName) ? null : new AssemblyName(assemblyName);
        }
    }
}