// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Messages.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Tests.TestServices;

public interface IReturnStringMessage : IRequest<string>;

public record OneReturnStringTypedMessage : IReturnStringMessage;

public record TwoReturnStringTypedMessage : IReturnStringMessage;

public record ReturnStringMessage : IRequest<string>;

public record ReturnIntMessage : IRequest<int>;