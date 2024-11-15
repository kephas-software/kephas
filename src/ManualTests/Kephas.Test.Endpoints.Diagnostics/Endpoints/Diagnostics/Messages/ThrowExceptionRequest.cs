// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowExceptionRequest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Test.Endpoints.Diagnostics.Messages;

public record ThrowExceptionRequest() : IRequest;