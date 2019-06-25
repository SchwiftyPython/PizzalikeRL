/// <summary>
/// For things that wanna know what's up in the world
/// </summary>
public interface ISubscriber
{
    //todo if we need additional params we can make dictionary
    void OnNotify(string eventName, object broadcaster, object parameter = null);
}
