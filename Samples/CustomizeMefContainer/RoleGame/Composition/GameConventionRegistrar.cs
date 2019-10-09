namespace RoleGame.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Mef.Conventions;
    using RoleGame.Services;

    public class GameConventionRegistrar : MefConventionsRegistrarBase
    {
        protected override void RegisterConventions(ConventionBuilder builder, IList<Type> candidateTypes, ICompositionRegistrationContext registrationContext)
        {
            builder
                .ForTypesDerivedFrom<IGameManager>()
                .Export(b => b.AsContractType<IGameManager>())
                .Shared(CompositionScopeNames.Default);

            builder
                .ForTypesDerivedFrom<IUser>()
                .Export(b => b.AsContractType<IUser>())
                .Shared(CompositionScopeNames.Default);
        }
    }
}