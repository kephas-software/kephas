// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionsSnapshot.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the options snapshot class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Configuration
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The options snapshot.
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    public class OptionsSnapshot<T> : OptionsManager<T>
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsSnapshot{T}"/> class.
        /// </summary>
        /// <param name="optionsFactory">The options factory.</param>
        public OptionsSnapshot(IOptionsFactory<T> optionsFactory)
            : base(optionsFactory)
        {
            // TODO until the https://github.com/dotnet/corefx/issues/40094 bug is fixed, make this constructor parameterless.
            // Options and OptionsSnapshot main purpose is to split the registration of the OptionsManager with two contracts,
            // as MEF does not like this scenario. For some reason it fails with SharingBoundary duplicate metadata registration.
        }
    }
}