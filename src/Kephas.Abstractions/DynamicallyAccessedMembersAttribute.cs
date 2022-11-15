// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicallyAccessedMembersAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETSTANDARD2_1

namespace Kephas
{
    using System;

    [Flags]
    public enum DynamicallyAccessedMemberTypes
    {
        None = 0,
        PublicParameterlessConstructor = 1,
        PublicConstructors = 3,
        NonPublicConstructors = 4,
        PublicMethods = 8,
        NonPublicMethods = 16, // 0x00000010
        PublicFields = 32, // 0x00000020
        NonPublicFields = 64, // 0x00000040
        PublicNestedTypes = 128, // 0x00000080
        NonPublicNestedTypes = 256, // 0x00000100
        PublicProperties = 512, // 0x00000200
        NonPublicProperties = 1024, // 0x00000400
        PublicEvents = 2048, // 0x00000800
        NonPublicEvents = 4096, // 0x00001000
        All = -1, // 0xFFFFFFFF
    }

    /// <summary>
    /// Indicates that certain members on a specified <see cref="Type"/> are accessed dynamically,
    /// for example through <see cref="System.Reflection"/>.
    /// </summary>
    /// <remarks>
    /// This allows tools to understand which members are being accessed during the execution
    /// of a program.
    ///
    /// This attribute is valid on members whose type is <see cref="Type"/> or <see cref="string"/>.
    ///
    /// When this attribute is applied to a location of type <see cref="string"/>, the assumption is
    /// that the string represents a fully qualified type name.
    ///
    /// If the attribute is applied to a method it's treated as a special case and it implies
    /// the attribute should be applied to the "this" parameter of the method. As such the attribute
    /// should only be used on instance methods of types assignable to System.Type (or string, but no methods
    /// will use it there).
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method,
        Inherited = false)]
    public sealed class DynamicallyAccessedMembersAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicallyAccessedMembersAttribute"/> class
        /// with the specified member types.
        /// </summary>
        /// <param name="memberTypes">The types of members dynamically accessed.</param>
        public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
        {
            MemberTypes = memberTypes;
        }

        /// <summary>
        /// Gets the <see cref="DynamicallyAccessedMemberTypes"/> which specifies the type
        /// of members dynamically accessed.
        /// </summary>
        public DynamicallyAccessedMemberTypes MemberTypes { get; }
    }
}

#endif