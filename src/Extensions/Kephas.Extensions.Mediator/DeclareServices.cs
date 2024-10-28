// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServices.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Extensions.Mediator;
using MediatR;

[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<>), ServiceType = typeof(IRequestHandlerBase), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IRequestHandler<,>), ServiceType = typeof(IRequestResponseHandlerBase), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IStreamRequestHandler<,>), ServiceType = typeof(IStreamRequestHandlerBase), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(INotificationHandler<>), ServiceType = typeof(INotificationHandlerBase), MetadataType = typeof(NotificationMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IPipelineBehavior<,>), ServiceType = typeof(IPipelineBehaviorBase), MetadataType = typeof(RequestResponseMetadata))]
[assembly: DeclareTransientServiceFamily(typeof(IStreamPipelineBehavior<,>), ServiceType = typeof(IStreamPipelineBehaviorBase), MetadataType = typeof(RequestResponseMetadata))]
