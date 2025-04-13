// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowExceptionHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;
using Kephas.Test.Endpoints.Diagnostics.Messages;
using MediatR;

namespace Kephas.Test.Endpoints.Diagnostics.Handlers;

public class ThrowExceptionHandler : IRequestHandler<ThrowExceptionRequest>
{
    public async Task Handle(ThrowExceptionRequest request, CancellationToken cancellationToken)
    {
        throw new AppException("Bad logic.");
    }
}