// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorHandlerResolverTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Kephas.DependencyInjection.Reflection;
using Kephas.Extensions.Mediator;
using Kephas.Extensions.Mediator.PipelineBehaviors;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Reflection;
using Kephas.Testing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Extensions.Tests.Mediator;

public class MediatorHandlerResolverTest() : IntegrationTestBase<MediatorHandlerResolverTest>("^Kephas\\.")
{
    private static readonly MethodInfo TryGetMatchingHandlerMethod =
        ReflectionHelper.GetGenericMethodOf(_ => ((IMediatorHandlerResolver)null!).TryGetMatchingHandler<string>(null!));
    private static readonly MethodInfo TryGetMatchingHandlerEnumerationMethod =
        ReflectionHelper.GetGenericMethodOf(_ => ((IMediatorHandlerResolver)null!).TryGetMatchingHandlerEnumeration<string>(null!));
    
    [Theory]
    [MemberData(nameof(GetSingleServiceData))]
    public void GetService_ReturnsSingleService(Type serviceType, Type expectedInstanceType)
    {
        var mediatorHandlerResolver = new MediatorHandlerResolver(ServiceProvider.GetRequiredService<IServiceInfoCollection>());
        var tryGetMatchingHandler = TryGetMatchingHandlerMethod.MakeGenericMethod(serviceType);
        var serviceInstance = tryGetMatchingHandler.Call(mediatorHandlerResolver, ServiceProvider);

        serviceInstance.Should().BeOfType(expectedInstanceType);
    }

    [Theory]
    [MemberData(nameof(GetMultipleServiceData))]
    public void GetService_ReturnsMultipleService(Type serviceType, IEnumerable<Type> expectedInstanceTypes)
    {
        var mediatorHandlerResolver = new MediatorHandlerResolver(ServiceProvider.GetRequiredService<IServiceInfoCollection>());
        var tryGetMatchingHandlerEnumeration = TryGetMatchingHandlerEnumerationMethod.MakeGenericMethod(serviceType.GenericTypeArguments[0]);
        var serviceInstance = (IEnumerable<object>?)tryGetMatchingHandlerEnumeration.Call(mediatorHandlerResolver, ServiceProvider);

        serviceInstance!.Select(i => i.GetType()).Should().Equal(expectedInstanceTypes);
    }
    
    public static TheoryData<Type, Type> GetSingleServiceData() =>
        new()
        {
            { typeof(IRequestHandler<OneReturnStringTypedMessage, string>), typeof(OneReturnStringTypedHandler) },
            { typeof(IRequestHandler<TwoReturnStringTypedMessage, string>), typeof(ReturnStringInterfaceHandler) },
            { typeof(IRequestHandler<ReturnStringMessage, string>), typeof(ReturnStringHandler) },
            { typeof(IRequestHandler<ReturnIntMessage, int>), typeof(ReturnIntHandler) },

            { typeof(IRequestHandler<OneReturnVoidTypedMessage>), typeof(OneReturnVoidTypedHandler) },
            { typeof(IRequestHandler<TwoReturnVoidTypedMessage>), typeof(ReturnVoidInterfaceHandler) },
            { typeof(IRequestHandler<ReturnVoidMessage>), typeof(ReturnVoidHandler) },
            { typeof(IRequestHandler<OtherVoidMessage>), typeof(OtherVoidHandler) },

            { typeof(IStreamRequestHandler<OneStreamReturnStringTypedMessage, string>), typeof(OneStreamReturnStringTypedHandler) },
            { typeof(IStreamRequestHandler<TwoStreamReturnStringTypedMessage, string>), typeof(StreamReturnStringInterfaceHandler) },
            { typeof(IStreamRequestHandler<StreamReturnStringMessage, string>), typeof(StreamReturnStringHandler) },
            { typeof(IStreamRequestHandler<StreamReturnIntMessage, int>), typeof(StreamReturnIntHandler) },
        };

    public static TheoryData<Type, IEnumerable<Type>> GetMultipleServiceData() =>
        new()
        {
            {
                typeof(IEnumerable<IPipelineBehavior<OneReturnStringTypedMessage, string>>),
                [
                    typeof(ReturnStringTypedBehavior),
                    typeof(LogPipelineBehavior<OneReturnStringTypedMessage, string>),
                    typeof(RequestValidationPipelineBehavior<OneReturnStringTypedMessage, string>),
                    typeof(OpenGenericBehavior<OneReturnStringTypedMessage, string>),
                    typeof(OneReturnStringTypedBehavior),
                    typeof(ReturnStringBehavior),
                ]
            },
            {
                typeof(IEnumerable<IPipelineBehavior<TwoReturnStringTypedMessage, string>>),
                [
                    typeof(ReturnStringTypedBehavior),
                    typeof(LogPipelineBehavior<TwoReturnStringTypedMessage, string>),
                    typeof(RequestValidationPipelineBehavior<TwoReturnStringTypedMessage, string>),
                    typeof(OpenGenericBehavior<TwoReturnStringTypedMessage, string>),
                    typeof(ReturnStringBehavior),
                ]
            },
            {
                typeof(IEnumerable<IPipelineBehavior<ReturnStringMessage, string>>),
                [
                    typeof(LogPipelineBehavior<ReturnStringMessage, string>),
                    typeof(RequestValidationPipelineBehavior<ReturnStringMessage, string>),
                    typeof(OpenGenericBehavior<ReturnStringMessage, string>),
                    typeof(ReturnStringBehavior),
                ]
            },
            {
                typeof(IEnumerable<IPipelineBehavior<ReturnIntMessage, int>>),
                [
                    typeof(LogPipelineBehavior<ReturnIntMessage, int>),
                    typeof(RequestValidationPipelineBehavior<ReturnIntMessage, int>),
                    typeof(OpenGenericBehavior<ReturnIntMessage, int>),
                    typeof(ReturnIntBehavior),
                ]
            },
            {
                typeof(IEnumerable<IStreamPipelineBehavior<StreamReturnIntMessage, int>>),
                [
                    typeof(StreamLogPipelineBehavior<StreamReturnIntMessage, int>),
                    typeof(StreamRequestValidationPipelineBehavior<StreamReturnIntMessage, int>),
                ]
            },

            // { typeof(IRequestHandler<OneReturnVoidTypedMessage>), typeof(OneReturnVoidTypedHandler) },
            // { typeof(IRequestHandler<TwoReturnVoidTypedMessage>), typeof(ReturnVoidInterfaceHandler) },
            // { typeof(IRequestHandler<ReturnVoidMessage>), typeof(ReturnVoidHandler) },
            // { typeof(IRequestHandler<OtherVoidMessage>), typeof(OtherVoidHandler) },
            //
            // { typeof(IStreamRequestHandler<OneStreamReturnStringTypedMessage, string>), typeof(OneStreamReturnStringTypedHandler) },
            // { typeof(IStreamRequestHandler<TwoStreamReturnStringTypedMessage, string>), typeof(StreamReturnStringInterfaceHandler) },
            // { typeof(IStreamRequestHandler<StreamReturnStringMessage, string>), typeof(StreamReturnStringHandler) },
            // { typeof(IStreamRequestHandler<StreamReturnIntMessage, int>), typeof(StreamReturnIntHandler) },
        };
}