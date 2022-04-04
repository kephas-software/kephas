namespace Kephas.Workflow.Activities;

using System.Threading;
using System.Threading.Tasks;
using Kephas.Threading.Tasks;
using Kephas.Workflow;

/// <summary>
/// Sets a variable value with the evaluated expression.
/// </summary>
public class SetActivity : FlowActivityBase, IParentScopeModifierActivity
{
    /// <summary>
    /// Gets or sets the variable name.
    /// </summary>
    /// <value>
    /// The variable name.
    /// </value>
    public string? Variable { get; set; }

    /// <summary>
    /// Gets or sets the activity providing the variable value.
    /// </summary>
    public IActivity? Expression { get; set; }

    /// <summary>
    /// Executes the operation asynchronously in the given context.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An object.
    /// </returns>
    public override async Task<object?> ExecuteAsync(IActivityContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(this.Variable))
        {
            throw new WorkflowException("The variable name cannot be empty.");
        }

        var varValue = this.Expression is null ? null : await this.ExecuteChildActivityAsync(this.Expression, context, this.Target, this.Arguments, cancellationToken).PreserveThreadContext();
        context.Scope[this.Variable] = varValue;
        return varValue;
    }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.ICloneable.Clone?view=netstandard-2.1">`ICloneable.Clone` on docs.microsoft.com</a></footer>
    public override IActivity Clone()
    {
        return new SetActivity
        {
            Variable = this.Variable,
            Expression = (this.Expression as ICloneable)?.Clone() as IActivity ?? this.Expression,
        };
    }
}