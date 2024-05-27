// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromReflectionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marks a type or a member being excluded from the model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Marks a type or a member being excluded from the reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromReflectionAttribute : Attribute
    {
    }
}
