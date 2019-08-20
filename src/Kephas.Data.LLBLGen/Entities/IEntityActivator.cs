// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityActivator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the illbl generate entity activator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities
{
    using Kephas.Activation;
    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for the entity activator.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IEntityActivator : IActivator
    {
    }
}