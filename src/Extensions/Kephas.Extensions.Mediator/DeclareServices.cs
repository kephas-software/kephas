// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Extensions.Mediator;
using MediatR;

[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<>), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<,>), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IStreamRequestHandler<,>), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(INotificationHandler<>),  MetadataType = typeof(NotificationMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IPipelineBehavior<,>), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IStreamPipelineBehavior<,>), MetadataType = typeof(RequestResponseMetadata))]
