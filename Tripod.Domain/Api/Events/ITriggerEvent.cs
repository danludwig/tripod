namespace Tripod
{
    public interface ITriggerEvent<in TEvent> where TEvent : IDefineEvent
    {
        void Trigger(TEvent e);
    }
}