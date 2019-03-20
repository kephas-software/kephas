// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppServiceType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IAppServiceType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Services.Reflection;

    /// <summary>
    /// Interface for application service.
    /// </summary>
    public interface IAppServiceType : IAppServiceInfo, IClassifier
    {
    }
}