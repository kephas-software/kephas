namespace Kephas.Data.Behaviors
{
  using System.Threading;
  using System.Threading.Tasks;

  public interface IAsyncPersistable
  {
    Task BeforePersistAsync(IDataContext context, CancellationToken cancellationToken = default(CancellationToken));

    Task AfterPersistAsync(IDataContext context, CancellationToken cancellationToken = default(CancellationToken));
  }
}