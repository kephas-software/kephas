// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlReadRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Interceptors;

namespace Kephas.Entities;

/// <summary>
/// The default implementation of <see cref="ISqlReadRepository"/>
/// </summary>
public class SqlReadRepository(
    ISqlEntityContextCollection entityContextCollection,
    IRepositoryOperationPipeline operationPipeline)
    : SqlReadRepositoryBase(entityContextCollection, operationPipeline), ISqlReadRepository;