namespace Shared.Interfaces
{
    public interface IClient<SIn, PIn, TOut>
    {
        ValueTask<TOut> Connect(SIn service, PIn proxy);
    }
}
