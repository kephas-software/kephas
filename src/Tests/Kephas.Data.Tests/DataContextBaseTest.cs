// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System.Linq;
    using System.Runtime.Remoting.Metadata.W3cXsd2001;

    using Kephas.Composition;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;

    using NUnit.Framework;

    using Telerik.JustMock;
    using Telerik.JustMock.Helpers;

    [TestFixture]
    public class DataContextBaseTest
    {
        [Test]
        public void CreateCommand()
        {
            var container = Mock.Create<ICompositionContext>();
            var findCmd = Mock.Create<IFindCommand<string>>();
            var factory = Mock.Create<IDataCommandFactory<IFindCommand<string>>>();
            factory
                .Arrange(f => f.CreateCommand(typeof(TestDataContext)))
                .Returns(findCmd);
            container
                .Arrange(c => c.GetExport<IDataCommandFactory<IFindCommand<string>>>(Arg.AnyString))
                .Returns(factory);

            var dataContext = new TestDataContext(container);
            var cmd = dataContext.CreateCommand<IFindCommand<string>>();
            Assert.AreSame(findCmd, cmd);
        }

        public class TestDataContext : DataContextBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataContextBase"/> class.
            /// </summary>
            public TestDataContext(ICompositionContext compositionContext = null) 
                : base(compositionContext ?? Mock.Create<ICompositionContext>())
            {
            }

            /// <summary>
            /// Gets a query over the entity type for the given query operationContext, if any is provided.
            /// </summary>
            /// <typeparam name="T">The entity type.</typeparam>
            /// <param name="queryOperationContext">Context for the query.</param>
            /// <returns>
            /// A query over the entity type.
            /// </returns>
            public override IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}