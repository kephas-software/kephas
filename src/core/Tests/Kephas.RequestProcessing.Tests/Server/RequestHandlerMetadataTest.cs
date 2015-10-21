// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandlerMetadataTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Test class for <see cref="RequestHandlerMetadata" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kephas.RequestProcessing.Server;
using NUnit.Framework;
using Telerik.JustMock;

namespace Kephas.RequestProcessing.Tests.Server
{
    /// <summary>
    /// Test class for <see cref="RequestHandlerMetadata"/>
    /// </summary>
    [TestFixture]
    public class RequestHandlerMetadataTest
    {
        [Test]
        public void RequestHandlerMetadata_metadata_null()
        {
            var instance = new RequestHandlerMetadata(null);
            var requestType = instance.RequestType;
            Assert.AreEqual(requestType, null);
        }

        [Test]
        public void RequestHandlerMetadata_metadata_valid()
        {
            var metadata = new Dictionary<string, object> { { RequestHandlerMetadata.RequestTypeKey, typeof(string) } };
            var instance = new RequestHandlerMetadata(metadata);
            var requestType = instance.RequestType;
            Assert.AreEqual(requestType, typeof(string));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void RequestHandlerMetadata_metadata_type_mismatch()
        {
            var metadata = new Dictionary<string, object> { { RequestHandlerMetadata.RequestTypeKey, "hello" } };
            var instance = new RequestHandlerMetadata(metadata);
        }

        [Test]
        public void RequestHandlerMetadata_metadata_type()
        {
            var instance = new RequestHandlerMetadata(typeof(int));
            var result = instance.RequestType;
            Assert.AreEqual(typeof(int), result);
        }
    }
}
