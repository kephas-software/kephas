// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOperation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using Kephas.Services;

    /// <summary>
    /// Defines the contract of an executable operation.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Executes the operation in the given context.
        /// </summary>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// An object.
        /// </returns>
        object Execute(IContext context = null);
    }
}