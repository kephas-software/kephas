namespace Kephas.Data.Commands
{
  using System.Threading;
  using System.Threading.Tasks;

  public interface IDataCommand
  {
    Task ExecuteAsync(IDataContext context, CancellationToken cancellationToken = default(CancellationToken));
  }
}