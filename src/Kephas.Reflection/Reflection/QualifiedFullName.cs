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
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// A qualified full name.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    internal class QualifiedFullName
    {
        private const char NamespaceSeparator = '.';
        private const char TypeDefinitionSeparator = ',';
        private const char GenericOpeningBracket = '[';
        private const char GenericClosingBracket = ']';
        private const int AssemblyNameIndex = 1;

        private Lazy<QualifiedFullName?>? genericDefinition;
        private Lazy<IEnumerable<QualifiedFullName>>? genericArguments;

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
        /// Initializes a new instance of the <see cref="QualifiedFullName" /> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        protected QualifiedFullName(string typeName, AssemblyName? assemblyName)
        {
            this.SetTypeName(typeName);
            this.AssemblyName = assemblyName;
        }

        /// <summary>
        /// Gets the name part from the type name.
        /// </summary>
        public string Name { get; private set; } = null!;

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
        public string TypeName { get; private set; } = null!;

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

        /// <summary>
        /// Gets the generic type definition.
        /// </summary>
        /// <returns>A <see cref="QualifiedFullName"/> containing the generic type definition.</returns>
        public QualifiedFullName? GetGenericTypeDefinition()
        {
            this.genericDefinition ??= new Lazy<QualifiedFullName?>(() =>
            {
                var genericStart = this.TypeName.IndexOf(GenericOpeningBracket);
                return genericStart < 0
                    ? null
                    : new QualifiedFullName(this.TypeName[..genericStart], this.AssemblyName);
            });

            return this.genericDefinition.Value;
        }

        /// <summary>
        /// Gets the generic type arguments if the type is a constructed generic.
        /// </summary>
        /// <returns>the generic type arguments</returns>
        public IEnumerable<QualifiedFullName> GetGenericTypeArguments()
        {
            this.genericArguments ??= new Lazy<IEnumerable<QualifiedFullName>>(
                () => ComputeGenericTypeArguments(this.TypeName));

            return this.genericArguments.Value;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A string that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.AssemblyName is null
                ? this.TypeName
                : $"{this.TypeName}, {this.AssemblyName}";
        }

        private static IEnumerable<QualifiedFullName> ComputeGenericTypeArguments(string typeName)
        {
            var genericStart = typeName.IndexOf(GenericOpeningBracket);
            if (genericStart < 0)
            {
                throw new InvalidOperationException($"The type '{typeName}' is not a generic type.");
            }

            var genericEnd = typeName.LastIndexOf(GenericClosingBracket);
            if (genericEnd < 0)
            {
                throw new ArgumentException($"The type name '{typeName}' is malformed, mismatched opening and closing generic bracket.");
            }

            var crt = typeName.IndexOf(GenericOpeningBracket, genericStart + 1);
            var argStart = crt + 1;
            while (crt >= 0)
            {
                crt++;
                var open = 1;
                var closed = 0;
                while (open > closed && crt < genericEnd)
                {
                    switch (typeName[crt++])
                    {
                        case GenericOpeningBracket: open++; break;
                        case GenericClosingBracket: closed++; break;
                    }
                }

                if (open > closed)
                {
                    throw new ArgumentException($"The type name '{typeName}' is malformed, mismatched opening and closing generic bracket.");
                }

                yield return new QualifiedFullName(typeName[argStart..(crt - 1)]);

                crt = typeName.IndexOf(GenericOpeningBracket, crt);
                argStart = crt + 1;
            }
        }

        private static (string typeName, string? assemblyName) ParseParts(string qualifiedFullName)
        {
            qualifiedFullName = qualifiedFullName ?? throw new ArgumentNullException(nameof(qualifiedFullName));

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

            var typeName = qualifiedFullName[..typeNameLength];

            // the trailing parts contain the assembly name, version, and public key token
            var trailingParts = qualifiedFullName[typeNameLength..]
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
                var typeNameNoGeneric = typeName[..indexOfGeneric];
                indexOfDot = typeNameNoGeneric.LastIndexOf(NamespaceSeparator);
            }
            else
            {
                indexOfDot = typeName.LastIndexOf(NamespaceSeparator);
            }

            if (indexOfDot >= 0)
            {
                this.Namespace = typeName[..indexOfDot];
                this.Name = typeName[(indexOfDot + 1)..];
            }
            else
            {
                this.Namespace = null;
                this.Name = typeName;
            }
        }

        private string GetDebuggerDisplay()
        {
            return $"{this} ({this.GetType()})";
        }
    }
}