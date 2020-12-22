using System.Threading.Tasks;

namespace Pinger.Config
{
    public interface IConfig<T, U>
    {
        T ConfigSource { get; }
        U Read();
        void Write(U configData);
        bool CreateDefaultConfig(U configData);
    }
}
