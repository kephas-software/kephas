// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludeFromServicesAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Indicates that a specific service should be excluded from composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.AttributedModel
{
    using System;

    /// <summary>
    /// Marks an eligible class for an application service implementation to be excluded from dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromServicesAttribute : Attribute
    {
    }
}