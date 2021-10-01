// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IToken.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Defines a token.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Gets the token name.
        /// </summary>
        public string TokenName { get; }
    }
}