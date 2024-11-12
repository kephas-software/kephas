// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlWriteRepository.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Interceptors;

namespace Kephas.Entities;

/// <summary>
/// The default implementation of <see cref="ISqlWriteRepository"/>
/// </summary>
public class SqlWriteRepository(
    ISqlEntityContextSelector entityContextSelector,
    IRepositoryOperationPipeline operationPipeline)
    : SqlWriteRepositoryBase(entityContextSelector, operationPipeline), ISqlWriteRepository;