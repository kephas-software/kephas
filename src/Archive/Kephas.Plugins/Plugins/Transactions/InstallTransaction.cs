// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallTransaction.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the install transaction class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Transactions
{
    /// <summary>
    /// An install transaction.
    /// </summary>
    public class InstallTransaction : TransactionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallTransaction"/> class.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        public InstallTransaction(PluginData pluginData)
            : base(pluginData)
        {
        }

        /// <summary>
        /// Gets the command key part.
        /// </summary>
        /// <returns>
        /// The command key part.
        /// </returns>
        protected override string GetCommandKeyPart() => $"-inst{UndoCommandBase.KeyPart}";
    }
}
