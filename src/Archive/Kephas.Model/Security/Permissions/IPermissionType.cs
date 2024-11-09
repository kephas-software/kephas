// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPermissionType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Security.Permissions
{
    using Kephas.Security.Permissions.Reflection;

    /// <summary>
    /// Interface for permission type.
    /// </summary>
    public interface IPermissionType : IClassifier, IPermissionInfo
    {
    }
}