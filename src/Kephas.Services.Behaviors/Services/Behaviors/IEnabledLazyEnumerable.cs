// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEnabledLazyEnumerable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Service enumerating enabled lazy services of type <typeparamref name="TContract"/>.
    /// </summary>
    /// <typeparam name="TContract">The service contract type.</typeparam>
    /// <typeparam name="TMetadata">The service metadata type.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IEnabledLazyEnumerable<TContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata>
        : IEnumerable<Lazy<TContract, TMetadata>>
    {
    }
}