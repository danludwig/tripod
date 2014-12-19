namespace Tripod
{
    public interface IProcessEvents
    {
        void Process(IDefineEvent e);
    }
}