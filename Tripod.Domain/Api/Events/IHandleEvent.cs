namespace Tripod
{
    public interface IHandleEvent<in TEvent> where TEvent : IDefineEvent
    {
        void Handle(TEvent e);
    }
}