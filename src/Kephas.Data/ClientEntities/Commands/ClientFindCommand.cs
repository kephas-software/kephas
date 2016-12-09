// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFindCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client find command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.ClientEntities.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Find command implementation for a <see cref="ClientDataContext"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class ClientFindCommand<T> : FindCommandBase<ClientDataContext, T>
        where T : class
    {
    }
}