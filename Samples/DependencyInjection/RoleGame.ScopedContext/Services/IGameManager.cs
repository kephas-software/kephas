// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGameManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace RoleGame.Services
{
    [ScopedAppServiceContract]
    public interface IGameManager
    {
        IUser User { get; }
    }
}