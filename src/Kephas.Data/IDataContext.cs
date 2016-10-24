namespace Kephas.Data
{
  using Kephas.Services;

  public interface IDataContext : IContext
  {
    IDataRepository Repository { get; }
  }
}