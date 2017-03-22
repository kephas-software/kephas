// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoCreateEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo create entity command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands;

    /// <summary>
    /// Command for creating an entity for <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoCreateEntityCommand : CreateEntityCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCreateEntityCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public MongoCreateEntityCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
        }
    }
}