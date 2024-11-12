// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Entities;

// Declare the entity context using this construct because, typically, the entity context classes are generated,
// and they will not go through the analyzer again to generate the declarations.
[assembly: DeclareScopedServiceFamily(
    typeof(IEntityContext<>),
    ServiceType = typeof(IEntityContext),
    MetadataType = typeof(EntityContextMetadata))]