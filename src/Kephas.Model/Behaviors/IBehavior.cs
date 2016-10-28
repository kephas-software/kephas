// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Behaviors
{
  public interface IBehavior<TEntity>
  {
    // behaviors provide methods used in conjunction with the data behaviors
    // the convention is bt name
    // For IAsyncInitializable, the behavior will have an InitializeAsync method
    // For IAsyncPersistable, the behavior will have BeforePersistAsync an AfterPersistAsync methods
  }
}