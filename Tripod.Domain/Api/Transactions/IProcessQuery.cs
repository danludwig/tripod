namespace Tripod
{
    public interface IProcessQuery
    {
        TResult Execute<TResult>(IDefineQuery<TResult> query);
    }
}
