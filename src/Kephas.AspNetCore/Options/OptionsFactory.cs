// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionsFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the options factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Options
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Options;

    /// <summary>
    /// The options factory.
    /// </summary>
    /// <typeparam name="T">The options type.</typeparam>
    public class OptionsFactory<T> : Microsoft.Extensions.Options.OptionsFactory<T>
        where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsFactory{T}"/> class.
        /// </summary>
        public OptionsFactory()
            : base(
                AmbientServices.Instance.CompositionContainer.GetExports<IConfigureOptions<T>>(),
                AmbientServices.Instance.CompositionContainer.GetExports<IPostConfigureOptions<T>>())
        {
            // TODO until the https://github.com/dotnet/corefx/issues/40094 bug is fixed, make this constructor parameterless.
            // TODO remove this class when the bug is corrected and its registration.
        }
    }
}