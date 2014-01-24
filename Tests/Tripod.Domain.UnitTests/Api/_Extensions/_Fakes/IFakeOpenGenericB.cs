namespace Tripod
{
    public interface IFakeOpenGenericB<out T>
    {
        [UsedImplicitly]
        T Property { get; }
    }
}