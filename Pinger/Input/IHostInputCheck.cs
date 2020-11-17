namespace Pinger.Input
{
    public interface IHostInputCheck
    {
        bool Check(string host, string formatPattern);
    }
}
