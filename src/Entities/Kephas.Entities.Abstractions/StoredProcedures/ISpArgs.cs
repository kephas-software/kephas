// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpArgs.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities.StoredProcedures;

/// <summary>
/// Marks stored procedure arguments used in <see cref="IReadRepository.Query{T}(ISpArgs, bool)"/>.
/// </summary>
public interface ISpArgs;