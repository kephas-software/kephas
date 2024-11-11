// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbWriteRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities;

/// <summary>
/// The default implementation of <see cref="IDbWriteRepository"/>
/// </summary>
public class DbWriteRepository(IDbEntityContextSelector entityContextSelector)
    : DbWriteRepositoryBase(entityContextSelector), IDbWriteRepository;