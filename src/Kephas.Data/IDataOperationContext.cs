// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataOperationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataOperationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
  using Kephas.Services;

  /// <summary>
  /// Contract for data operations contexts.
  /// </summary>
  public interface IDataOperationContext : IContext
  {
    /// <summary>
    /// Gets the dataContext.
    /// </summary>
    /// <value>
    /// The dataContext.
    /// </value>
    IDataContext DataContext { get; }
  }
}