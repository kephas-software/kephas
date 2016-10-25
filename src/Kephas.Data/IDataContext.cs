// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
  using Kephas.Services;

  /// <summary>
  /// Contract for data operations contexts.
  /// </summary>
  public interface IDataContext : IContext
  {
    /// <summary>
    /// Gets the repository.
    /// </summary>
    /// <value>
    /// The repository.
    /// </value>
    IDataRepository Repository { get; }
  }
}