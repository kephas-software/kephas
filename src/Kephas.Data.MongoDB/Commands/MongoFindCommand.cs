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
    /// <typeparam name="T">The type of the entity to be found.</typeparam>
    public class MongoFindCommand<T> : FindCommandBase<MongoDataContext, T>
        where T : class
    {
    }
}