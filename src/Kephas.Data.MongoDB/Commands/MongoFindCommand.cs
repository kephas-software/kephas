// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoFindCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo find command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Command for finding an entity for <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoFindCommand : FindCommandBase
    {
    }
}