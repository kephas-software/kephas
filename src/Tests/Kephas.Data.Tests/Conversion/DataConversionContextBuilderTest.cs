// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConversionContextBuilderTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        public void Constructor_DataConversionService()
        {
            var service = Substitute.For<IDataConversionService>();
            var builder = new DataConversionContextBuilder(service);

            Assert.IsNotNull(builder.ConversionContext);
            Assert.AreSame(service, builder.ConversionContext.DataConversionService);
        }

        [Test]
        public void Constructor_DataConversionService_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DataConversionContextBuilder((IDataConversionService)null));
        }

        [Test]
        public void Merge()
        {
            var serviceSource = Substitute.For<IDataConversionService>();
            var serviceTarget = Substitute.For<IDataConversionService>();

            var contextSource = new DataConversionContextBuilder(serviceSource)
                                    .WithTargetDataContext(Substitute.For<IDataContext>())
                                    .WithRootTargetType(typeof(string))
                                    .WithSourceDataContext(Substitute.For<IDataContext>())
                                    .WithRootSourceType(typeof(bool))
                                    .ThrowOnError(false)
                                    .ConversionContext;

            var contextTarget = new DataConversionContextBuilder(serviceTarget)
                                    .Merge(contextSource)
                                    .ConversionContext;

            Assert.AreSame(serviceTarget, contextTarget.DataConversionService);
            Assert.AreSame(serviceTarget.AmbientServices, contextTarget.AmbientServices);
            Assert.AreSame(contextSource.TargetDataContext, contextTarget.TargetDataContext);
            Assert.AreSame(contextSource.RootTargetType, contextTarget.RootTargetType);
            Assert.AreSame(contextSource.SourceDataContext, contextTarget.SourceDataContext);
            Assert.AreSame(contextSource.RootSourceType, contextTarget.RootSourceType);
            Assert.AreEqual(contextSource.ThrowOnError, contextTarget.ThrowOnError);
        }
    }
}