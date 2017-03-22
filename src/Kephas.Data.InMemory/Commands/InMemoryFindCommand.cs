// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryFindCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the client find command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Find command implementation for a <see cref="InMemoryDataContext"/>.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryFindCommand : FindCommandBase
    {
    }
}