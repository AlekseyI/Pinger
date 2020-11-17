namespace Pinger.Input
{
    public interface IHostInputParse
    {
        (string, int) Parse(string host);
    }
}
