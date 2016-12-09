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

    using Kephas.Composition;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextBaseTest
    {
        [Test]
        public void CreateCommand()
        {
            var container = Substitute.For<ICompositionContext>();
            var findCmd = Substitute.For<IFindCommand<IDataContext, string>>();
            var factory = Substitute.For<IDataCommandFactory<IFindCommand<IDataContext, string>>>();
            factory.GetCommandFactory(typeof(TestDataContext)).Returns(() => findCmd);
            container.GetExport<IDataCommandFactory<IFindCommand<IDataContext, string>>>(Arg.Any<string>()).Returns(factory);

            var dataContext = new TestDataContext(compositionContext: container);
            var cmd = dataContext.CreateCommand<IFindCommand<IDataContext, string>>();
            Assert.AreSame(findCmd, cmd);
        }

        public class TestDataContext : DataContextBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DataContextBase"/> class.
            /// </summary>
            public TestDataContext(IAmbientServices ambientServices = null, ICompositionContext compositionContext = null) 
                : base(ambientServices ?? Substitute.For<IAmbientServices>(), compositionContext ?? Substitute.For<ICompositionContext>())
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