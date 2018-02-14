// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndpointTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the endpoint test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using System;

    using Kephas.Messaging.Distributed;

    using NUnit.Framework;

    [TestFixture]
    public class EndpointTest
    {
        [Test]
        public void Endpoint_url()
        {
            var endpoint = new Endpoint(new Uri("app://./app-id/app-instance-id/endpoint-id"));

            Assert.AreEqual("app-id", endpoint.AppId);
            Assert.AreEqual("app-instance-id", endpoint.AppInstanceId);
            Assert.AreEqual("endpoint-id", endpoint.EndpointId);
        }

        [Test]
        public void Endpoint_app_fragments()
        {
            var endpoint = new Endpoint("app-id", "app-instance-id", "endpoint-id");

            Assert.AreEqual(new Uri("app://./app-id/app-instance-id/endpoint-id"), endpoint.Url);
        }

        [Test]
        public void Endpoint_app_id()
        {
            var endpoint = new Endpoint("app-id");

            Assert.AreEqual(new Uri("app://./app-id//"), endpoint.Url);
        }

        [Test]
        public void Endpoint_app_instance_id()
        {
            var endpoint = new Endpoint(appInstanceId: "app-instance-id");

            Assert.AreEqual(new Uri("app://.//app-instance-id/"), endpoint.Url);
        }

        [Test]
        public void Endpoint_endpoint_id()
        {
            var endpoint = new Endpoint(endpointId: "endpoint-id");

            Assert.AreEqual(new Uri("app://.///endpoint-id"), endpoint.Url);
        }

        [Test]
        public void Url_app_fragments()
        {
            var endpoint = new Endpoint { Url = new Uri("app://./app-id/app-instance-id/endpoint-id") };

            Assert.AreEqual("app-id", endpoint.AppId);
            Assert.AreEqual("app-instance-id", endpoint.AppInstanceId);
            Assert.AreEqual("endpoint-id", endpoint.EndpointId);
        }

        [Test]
        public void Url_app_id()
        {
            var endpoint = new Endpoint { Url = new Uri("app://./app-id//") };

            Assert.AreEqual("app-id", endpoint.AppId);
            Assert.IsNull(endpoint.AppInstanceId);
            Assert.IsNull(endpoint.EndpointId);
        }

        [Test]
        public void Url_app_instance_id()
        {
            var endpoint = new Endpoint { Url = new Uri("app://.//app-instance-id/") };

            Assert.IsNull(endpoint.AppId);
            Assert.AreEqual("app-instance-id", endpoint.AppInstanceId);
            Assert.IsNull(endpoint.EndpointId);
        }

        [Test]
        public void Url_endpoint_id()
        {
            var endpoint = new Endpoint { Url = new Uri("app://.///endpoint-id") };

            Assert.IsNull(endpoint.AppId);
            Assert.IsNull(endpoint.AppInstanceId);
            Assert.AreEqual("endpoint-id", endpoint.EndpointId);
        }
    }
}