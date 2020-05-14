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
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A qualified full name.
    /// </summary>
    internal class QualifiedFullName
    {
        private const char NamespaceSeparator = '.';
        private const char TypeDefinitionSeparator = ',';
        private const char GenericOpeningBracket = '[';
        private const char GenericClosingBracket = ']';
        private const int AssemblyNameIndex = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedFullName"/> class.
        /// </summary>
        /// <param name="qualifiedFullName">The qualified full name.</param>
        public QualifiedFullName(string qualifiedFullName)
            : this(ParseParts(qualifiedFullName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedFullName"/> class.
        /// </summary>
        /// <param name="parts">The qualified full name parts.</param>
        protected QualifiedFullName((string typeName, string? assemblyName) parts)
        {
            this.SetTypeName(parts.typeName);
            this.AssemblyName = GetAssemblyName(parts.assemblyName);
        }

        /// <summary>
        /// Gets the name part from the type name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the namespace part from the type name.
        /// </summary>
        public string? Namespace { get; private set; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public AssemblyName? AssemblyName { get; }

        /// <summary>
        /// Parses the provided string and returns the <see cref="QualifiedFullName"/> instance.
        /// </summary>
        /// <param name="qualifiedFullName">The qualified full name.</param>
        /// <returns>The <see cref="QualifiedFullName"/> instance.</returns>
        public static QualifiedFullName Parse(string qualifiedFullName)
        {
            return new QualifiedFullName(ParseParts(qualifiedFullName));
        }

        private static (string typeName, string? assemblyName) ParseParts(string qualifiedFullName)
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
                return (qualifiedFullName, null);
            }

            var typeName = qualifiedFullName.Substring(0, typeNameLength);

            // the trailing parts contain the assembly name, version, and public key token
            var trailingParts = qualifiedFullName.Substring(typeNameLength)
                .Split(TypeDefinitionSeparator);

            if (trailingParts.Length <= AssemblyNameIndex)
            {
                return (typeName, null);
            }

            return (typeName, trailingParts[AssemblyNameIndex]);
        }

        private static AssemblyName? GetAssemblyName(string? rawAssemblyName)
        {
            var assemblyName = rawAssemblyName?.Trim();
            return string.IsNullOrWhiteSpace(assemblyName) ? null : new AssemblyName(assemblyName);
        }

        private void SetTypeName(string typeName)
        {
            this.TypeName = typeName;
            var indexOfGeneric = typeName.IndexOf(GenericOpeningBracket);
            int indexOfDot;
            if (indexOfGeneric >= 0)
            {
                var typeNameNoGeneric = typeName.Substring(0, indexOfGeneric);
                indexOfDot = typeNameNoGeneric.LastIndexOf(NamespaceSeparator);
            }
            else
            {
                indexOfDot = typeName.LastIndexOf(NamespaceSeparator);
            }

            if (indexOfDot >= 0)
            {
                this.Namespace = typeName.Substring(0, indexOfDot);
                this.Name = typeName.Substring(indexOfDot + 1);
            }
            else
            {
                this.Namespace = null;
                this.Name = typeName;
            }
        }
    }
}