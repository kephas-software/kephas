namespace Kephas.Data.Behaviors
{
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  public interface IAsyncAggregatable
  {
    Task<IEnumerable<object>> GetAggregationGraphAsync(
      IDataContext context,
      CancellationToken cancellationToken = default(CancellationToken));
  }
}