// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetServicesHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kephas.Composition;
using Kephas.Services;
using Kephas.Services.Composition;

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Messaging;
    using Kephas.Reflection;

    /// <summary>
    /// Message handler for <see cref="GetServicesMessage"/>.
    /// </summary>
    public class GetServicesHandler : MessageHandlerBase<GetServicesMessage, GetServicesResponseMessage>
    {
        private static readonly MethodInfo GetServicesMetadataMethod =
            ReflectionHelper.GetMethodOf(_ => ((GetServicesHandler)null!).GetServicesMetadata<int>(true));

        private readonly ITypeResolver typeResolver;
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetServicesHandler"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="compositionContext">The composition context.</param>
        public GetServicesHandler(ITypeResolver typeResolver, ICompositionContext compositionContext)
        {
            this.typeResolver = typeResolver;
            this.compositionContext = compositionContext;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override Task<GetServicesResponseMessage> ProcessAsync(GetServicesMessage message, IMessagingContext context, CancellationToken token)
        {
            if (message.ContractType == null)
            {
                throw new ArgumentException($"The contract type is not specified.");
            }

            var contractType = this.typeResolver.ResolveType(message.ContractType, throwOnNotFound: false);
            if (contractType == null)
            {
                throw new ArgumentException($"The contract type '{message.ContractType}' was not found.");
            }

            var getServicesMetadata = GetServicesMetadataMethod.MakeGenericMethod(contractType);
            var appServicesMetadata = (IEnumerable<AppServiceMetadata>)getServicesMetadata.Call(this, message.Ordered);
            return Task.FromResult(new GetServicesResponseMessage
            {
                Services = appServicesMetadata.ToArray(),
                Message = $"Services for '{contractType}'.",
            });
        }

        private IEnumerable<AppServiceMetadata> GetServicesMetadata<TContract>(bool ordered)
        {
            var factories = this.compositionContext.GetExportFactories<TContract, AppServiceMetadata>();
            return ordered
                ? factories.Order().Select(f => f.Metadata)
                : factories.Select(f => f.Metadata);
        }
    }
}