// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Domains.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Ontology;

namespace Kephas.Tests.TestServices;

public class CoreDomain(IServiceProvider serviceProvider) : DomainBase(serviceProvider);

[DomainDependencies<CoreDomain>]
public class CrmDomain(IServiceProvider serviceProvider) : DomainBase(serviceProvider);

[DomainDependencies<CoreDomain, CrmDomain>]
public class ContractingDomain(IServiceProvider serviceProvider) : DomainBase(serviceProvider);

[DomainDependencies<ContractingDomain, CrmDomain, CoreDomain>]
public class BIDomain(IServiceProvider serviceProvider) : DomainBase(serviceProvider);
