// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromInjectionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Indicates that a specific service should be excluded from composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Injection.AttributedModel
{
    /// <summary>
    /// Marks an eligible class for an application service implementation to be excluded from dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromInjectionAttribute : Attribute
    {
    }
}