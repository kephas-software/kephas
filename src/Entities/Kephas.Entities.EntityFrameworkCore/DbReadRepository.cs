// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbReadRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Entities;

/// <summary>
/// The default implementation of <see cref="IDbReadRepository"/>
/// </summary>
public class DbReadRepository(IDbEntityContextSelector entityContextSelector)
    : DbReadRepositoryBase(entityContextSelector), IDbReadRepository;