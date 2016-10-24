namespace Kephas.Data.Behaviors
{
  using System.Threading;
  using System.Threading.Tasks;

  public interface IAsyncInitializable
  {
    Task InitializeAsync(IDataContext context, CancellationToken cancellationToken = default(CancellationToken));
  }
}