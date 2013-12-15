namespace Tripod
{
    public interface IProcessQueries
    {
        TResult Execute<TResult>(IDefineQuery<TResult> query);
    }
}
