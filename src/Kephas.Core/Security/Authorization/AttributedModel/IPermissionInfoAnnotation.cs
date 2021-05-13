// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionInfoAnnotation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization.AttributedModel
{
    using Kephas.Reflection;
    using Kephas.Security.Authorization.Reflection;

    /// <summary>
    /// Marker interface for permission info annotations.
    /// </summary>
    public interface IPermissionInfoAnnotation : IScoped, IToken
    {
    }
}