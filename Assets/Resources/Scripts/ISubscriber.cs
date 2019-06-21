public interface ISubscriber
{
    void OnNotify(string eventName, object broadcaster);
}
