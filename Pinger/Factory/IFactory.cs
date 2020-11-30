namespace Pinger.Factory
{
    public interface IFactory<in T, out U>
    {
        U GetInstance(T param);
    }
}
