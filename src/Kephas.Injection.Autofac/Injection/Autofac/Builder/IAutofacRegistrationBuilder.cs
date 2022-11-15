// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutofacRegistrationBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac.Builder
{
    using Kephas.Injection.Builder;

    /// <summary>
    /// Autofac specific <see cref="IRegistrationBuilder"/>.
    /// </summary>
    public interface IAutofacRegistrationBuilder : IRegistrationBuilder
    {
        /// <summary>
        /// Builds the registration.
        /// </summary>
        void Build();
    }
}