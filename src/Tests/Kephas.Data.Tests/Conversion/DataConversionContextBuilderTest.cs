// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContextBuilderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data conversion context builder test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Conversion
{
    using System;

    using Kephas.Data.Conversion;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataConversionContextBuilderTest
    {
        [Test]
        public void Constructor_DataConversionContext()
        {
            var context = Substitute.For<IDataConversionContext>();
            var builder = new DataConversionContextBuilder(context);

            Assert.AreSame(context, builder.ConversionContext);
        }

        [Test]
        public void Constructor_DataConversionContext_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DataConversionContextBuilder((IDataConversionContext)null));
        }

        [Test]
        public void Constructor_DataSpace_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DataConversionContextBuilder((IDataSpace)null, Substitute.For<IDataConversionService>()));
        }

        [Test]
        public void Constructor_DataConversionService_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DataConversionContextBuilder(Substitute.For<IDataSpace>(), (IDataConversionService)null));
        }

        [Test]
        public void Constructor_DataConversionService()
        {
            var service = Substitute.For<IDataConversionService>();
            var dataSpace = Substitute.For<IDataSpace>();
            var builder = new DataConversionContextBuilder(dataSpace, service);

            Assert.IsNotNull(builder.ConversionContext);
            Assert.AreSame(dataSpace, builder.ConversionContext.DataSpace);
            Assert.AreSame(service, builder.ConversionContext.DataConversionService);
        }

        [Test]
        public void Merge()
        {
            var dataSpace = Substitute.For<IDataSpace>();
            var serviceSource = Substitute.For<IDataConversionService>();
            var serviceTarget = Substitute.For<IDataConversionService>();

            var contextSource = new DataConversionContextBuilder(dataSpace, serviceSource)
                                    .WithRootTargetType(typeof(string))
                                    .WithRootSourceType(typeof(bool))
                                    .ThrowOnError(false)
                                    .ConversionContext;

            var contextTarget = new DataConversionContextBuilder(dataSpace, serviceTarget)
                                    .Merge(contextSource)
                                    .ConversionContext;

            Assert.AreSame(serviceTarget, contextTarget.DataConversionService);
            Assert.AreSame(contextSource.DataSpace, contextTarget.DataSpace);
            Assert.AreSame(contextSource.RootTargetType, contextTarget.RootTargetType);
            Assert.AreSame(contextSource.RootSourceType, contextTarget.RootSourceType);
            Assert.AreEqual(contextSource.ThrowOnError, contextTarget.ThrowOnError);
        }
    }
}