namespace Pinger.Input
{
    public interface IHostPortInput : IHostInput
    {
        int Port { get; }
    }
}