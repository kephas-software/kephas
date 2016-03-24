namespace StartupConsole.Initialization
{
    using Kephas.Application;
    using Kephas.Behavior;
    using Kephas.Services.Behavior;

    public class GreenConsoleEnabledServiceBehaviorRule : EnabledServiceBehaviorRuleBase<IAppInitializer, GreenConsoleAppInitializer>
    {
        /// <summary>
        /// Gets the behavior value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The behavior value.
        /// </returns>
        public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IAppInitializer> context)
        {
            //TODO uncomment the next line and see how the application initialization changes
            //return BehaviorValue.True;
            return BehaviorValue.False;
        }
    }
}