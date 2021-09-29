// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutofacPartBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Conventions
{
    using Kephas.Injection.Conventions;

    /// <summary>
    /// Autofac specific <see cref="IPartBuilder"/>.
    /// </summary>
    public interface IAutofacPartBuilder : IPartBuilder
    {
        /// <summary>
        /// Builds the registration.
        /// </summary>
        void Build();
    }
}