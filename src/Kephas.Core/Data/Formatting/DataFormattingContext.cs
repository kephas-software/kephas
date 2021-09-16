// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataFormattingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.Formatting
{
    using Kephas.Services;

    /// <summary>
    /// A data formatting context.
    /// </summary>
    public class DataFormattingContext : Context, IDataFormattingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormattingContext"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="merge">Optional. True to merge the parent context into the new context.</param>
        public DataFormattingContext(IContext parentContext, bool merge = false)
            : base(parentContext, merge)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormattingContext"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public DataFormattingContext(IAmbientServices ambientServices)
            : base(ambientServices)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormattingContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public DataFormattingContext(IInjector injector)
            : base(injector)
        {
        }
    }
}