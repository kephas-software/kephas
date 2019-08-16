// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the options class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Options
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// An options.
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    public class Options<T> : OptionsManager<T>
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options{T}"/> class.
        /// </summary>
        public Options()
            : base(AmbientServices.Instance.CompositionContainer.GetExport<IOptionsFactory<T>>())
        {
            // TODO until the https://github.com/dotnet/corefx/issues/40094 bug is fixed, make this constructor parameterless.
            // Options and OptionsSnapshot main purpose is to split the registration of the OptionsManager with two contracts,
            // as MEF does not like this scenario. For some reason it fails with SharingBoundary duplicate metadata registration.
        }
    }
}