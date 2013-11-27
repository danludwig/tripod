namespace Tripod
{
    public interface IFakeOpenGenericB<out T>
    {
        T Property { get; }
    }
}