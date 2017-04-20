// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDeleteEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo delete entity command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Command for creating an entity for <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoDeleteEntityCommand : DeleteEntityCommandBase
    {
    }
}