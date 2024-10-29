// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Messages.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;

namespace Kephas.Extensions.Tests.TestServices;

public interface IReturnVoidMessageBase : IRequest;

public interface IReturnVoidMessage : IReturnVoidMessageBase;

public record OneReturnVoidTypedMessage : IReturnVoidMessage;

public record TwoReturnVoidTypedMessage : IReturnVoidMessage;

public record ReturnVoidMessage : IReturnVoidMessageBase;

public record NonDeclaredVoidMessage : IRequest;