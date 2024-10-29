// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Mediator;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Testing;
using MediatR;
using Xunit;

namespace Kephas.Extensions.Tests.Mediator;

public class MediatorServiceProviderTest() : IntegrationTestBase<MediatorServiceProviderTest>("^Kephas\\.")
{
    [Theory]
    [MemberData(nameof(GetSingleServiceData))]
    public void GetService_ReturnsSingleService(Type serviceType, Type expectedInstanceType, bool isDeclared)
    {
        var logger = Test.GetTestLogger<MediatorServiceProvider>();

        var mediatorServiceProvider = new MediatorServiceProvider(ServiceProvider, logger);
        var serviceInstance = mediatorServiceProvider.GetService(serviceType);

        serviceInstance.Should().BeOfType(expectedInstanceType);
        if (isDeclared)
        {
            logger.ToString().Should().BeEmpty();
        }
        else
        {
            logger.ToString().Should().StartWith($"Warning: Identified handler '{expectedInstanceType}' for service type '{serviceType}'. However, it is strongly recommended to use the declared base classes instead of MediatR interfaces, to allow use of the behaviors.");
        }
    }
    [Theory]
    [MemberData(nameof(GetMultipleServiceData))]
    public void GetService_ReturnsMultipleService(Type serviceType, IEnumerable<Type> expectedInstanceTypes, bool isDeclared)
    {
        var logger = Test.GetTestLogger<MediatorServiceProvider>();

        var mediatorServiceProvider = new MediatorServiceProvider(ServiceProvider, logger);
        var serviceInstance = (IEnumerable<object>)mediatorServiceProvider.GetService(serviceType);

        serviceInstance.Select(i => i.GetType()).Should().Equal(expectedInstanceTypes);
        if (isDeclared)
        {
            logger.ToString().Should().BeEmpty();
        }
        else
        {
            logger.ToString().Should().StartWith($"Warning: Identified handler '{serviceType}' for service type '{serviceType}'. However, it is strongly recommended to use the declared base classes instead of MediatR interfaces, to allow use of the behaviors.");
        }
    }
    
    public static TheoryData<Type, Type, bool> GetSingleServiceData()
    {
        return new TheoryData<Type, Type, bool>
        {
            { typeof(IRequestHandler<OneReturnStringTypedMessage, string>), typeof(OneReturnStringTypedHandler), true },
            { typeof(IRequestHandler<TwoReturnStringTypedMessage, string>), typeof(ReturnStringInterfaceHandler), true },
            { typeof(IRequestHandler<ReturnStringMessage, string>), typeof(ReturnStringHandler), true },
            { typeof(IRequestHandler<NonDeclaredMessage, int>), typeof(NonDeclaredHandler), false },

            { typeof(IRequestHandler<OneReturnVoidTypedMessage>), typeof(OneReturnVoidTypedHandler), true },
            { typeof(IRequestHandler<TwoReturnVoidTypedMessage>), typeof(ReturnVoidInterfaceHandler), true },
            { typeof(IRequestHandler<ReturnVoidMessage>), typeof(ReturnVoidHandler), true },
            { typeof(IRequestHandler<NonDeclaredVoidMessage>), typeof(NonDeclaredVoidHandler), false },

            { typeof(IStreamRequestHandler<OneStreamReturnStringTypedMessage, string>), typeof(OneStreamReturnStringTypedHandler), true },
            { typeof(IStreamRequestHandler<TwoStreamReturnStringTypedMessage, string>), typeof(StreamReturnStringInterfaceHandler), true },
            { typeof(IStreamRequestHandler<StreamReturnStringMessage, string>), typeof(StreamReturnStringHandler), true },
            { typeof(IStreamRequestHandler<StreamNonDeclaredMessage, int>), typeof(StreamNonDeclaredHandler), false },
        };
    }
    
    public static TheoryData<Type, IEnumerable<Type>, bool> GetMultipleServiceData()
    {
        return new TheoryData<Type, IEnumerable<Type>, bool>
        {
            {
                typeof(IEnumerable<IPipelineBehavior<OneReturnStringTypedMessage, string>>),
                [typeof(ReturnStringTypedBehavior), typeof(OpenGenericBehavior<OneReturnStringTypedMessage, string>), typeof(OneReturnStringTypedBehavior), typeof(ReturnStringBehavior)],
                true
            },
            {
                typeof(IEnumerable<IPipelineBehavior<TwoReturnStringTypedMessage, string>>),
                [typeof(ReturnStringTypedBehavior), typeof(OpenGenericBehavior<TwoReturnStringTypedMessage, string>), typeof(ReturnStringBehavior)],
                true
            },
            {
                typeof(IEnumerable<IPipelineBehavior<ReturnStringMessage, string>>),
                [typeof(OpenGenericBehavior<ReturnStringMessage, string>), typeof(ReturnStringBehavior)],
                true
            },
            {
                typeof(IEnumerable<IPipelineBehavior<NonDeclaredMessage, string>>),
                [typeof(OpenGenericBehavior<NonDeclaredMessage, int>), typeof(NonDeclaredBehavior)],
                true
            },

            // { typeof(IRequestHandler<OneReturnVoidTypedMessage>), typeof(OneReturnVoidTypedHandler), true },
            // { typeof(IRequestHandler<TwoReturnVoidTypedMessage>), typeof(ReturnVoidInterfaceHandler), true },
            // { typeof(IRequestHandler<ReturnVoidMessage>), typeof(ReturnVoidHandler), true },
            // { typeof(IRequestHandler<NonDeclaredVoidMessage>), typeof(NonDeclaredVoidHandler), false },
            //
            // { typeof(IStreamRequestHandler<OneStreamReturnStringTypedMessage, string>), typeof(OneStreamReturnStringTypedHandler), true },
            // { typeof(IStreamRequestHandler<TwoStreamReturnStringTypedMessage, string>), typeof(StreamReturnStringInterfaceHandler), true },
            // { typeof(IStreamRequestHandler<StreamReturnStringMessage, string>), typeof(StreamReturnStringHandler), true },
            // { typeof(IStreamRequestHandler<StreamNonDeclaredMessage, int>), typeof(StreamNonDeclaredHandler), false },
        };
    }
}