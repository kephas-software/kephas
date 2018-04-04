// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataCommandBuilder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataCommandBuilder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Builders
{
  /// <summary>
  /// Interface for data command builder.
  /// </summary>
  public interface IDataCommandBuilder
  {
    /// <summary>
    /// Gets the constructed command.
    /// </summary>
    /// <value>
    /// The constructed command.
    /// </value>
    IDataCommand Command { get; }
  }
}