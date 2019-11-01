
namespace Metaprogramming.Matrix
{
    public interface ISize2
    {
        int X { get; }
        int Y { get; }
    }

    public interface IStorageLayout<TSize2, T>
        where TSize2 : struct, ISize2
    {
        void Initialize();
        void Set(int x, int y, T value);
        T Get(int x, int y);
    }

    public interface IStorageLayoutCreate<TSize2, T>
        where TSize2 : struct, ISize2
    {
        TStorage Create<TStorage>() where TStorage : IStorageLayoutCreate<TSize2, T>;
       
        void Set(int x, int y, T value);
        T Get(int x, int y);
    }
}
