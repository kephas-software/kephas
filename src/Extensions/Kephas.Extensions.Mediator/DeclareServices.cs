// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Extensions.Mediator;
using MediatR;

[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<>), ServiceType = typeof(IRequestHandlerBase), MetadataType = typeof(RequestHandlerMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<,>), ServiceType = typeof(IRequestResponseHandlerBase), MetadataType = typeof(RequestHandlerMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IStreamRequestHandler<,>), ServiceType = typeof(IStreamRequestHandlerBase), MetadataType = typeof(RequestHandlerMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(INotificationHandler<>), ServiceType = typeof(INotificationHandlerBase), MetadataType = typeof(NotificationHandlerMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IPipelineBehavior<,>), ServiceType = typeof(IPipelineBehaviorBase), MetadataType = typeof(RequestHandlerMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IStreamPipelineBehavior<,>), ServiceType = typeof(IStreamPipelineBehaviorBase), MetadataType = typeof(RequestHandlerMetadata))]
