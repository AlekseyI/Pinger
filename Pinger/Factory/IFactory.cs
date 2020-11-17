namespace Pinger.Factory
{
    public interface IFactory<T, out U>
    {
        U GetInstance(T param);
    }
}
