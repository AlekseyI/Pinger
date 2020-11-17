namespace Pinger.Response
{
    public interface IPingCodeResponse : IPingResponse
    {
        int Code { get; }
    }
}
