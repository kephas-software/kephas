// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromSerializationAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;

    /// <summary>
    /// Attribute for marking items ignored during serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ExcludeFromSerializationAttribute : Attribute
    {
    }
}