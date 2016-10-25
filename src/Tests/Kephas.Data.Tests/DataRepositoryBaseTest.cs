// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRepositoryBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data repository base test class.
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
    public class DataRepositoryBaseTest
    {
        [Test]
        public void CreateCommand()
        {
            var container = Mock.Create<ICompositionContext>();
            var findCmd = Mock.Create<IFindCommand<string>>();
            var factory = Mock.Create<IDataCommandFactory<IFindCommand<string>>>();
            factory
                .Arrange(f => f.CreateCommand(typeof(TestDataRepository)))
                .Returns(findCmd);
            container
                .Arrange(c => c.GetExport<IDataCommandFactory<IFindCommand<string>>>(Arg.AnyString))
                .Returns(factory);

            var repository = new TestDataRepository(container);
            var cmd = repository.CreateCommand<IFindCommand<string>>();
            Assert.AreSame(findCmd, cmd);
        }

        public class TestDataRepository : DataRepositoryBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataRepositoryBase"/> class.
            /// </summary>
            public TestDataRepository(ICompositionContext compositionContext = null) 
                : base(compositionContext ?? Mock.Create<ICompositionContext>())
            {
            }

            /// <summary>
            /// Gets a query over the entity type for the given query context, if any is provided.
            /// </summary>
            /// <typeparam name="T">The entity type.</typeparam>
            /// <param name="queryContext">Context for the query.</param>
            /// <returns>
            /// A query over the entity type.
            /// </returns>
            public override IQueryable<T> Query<T>(IQueryContext queryContext = null)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}