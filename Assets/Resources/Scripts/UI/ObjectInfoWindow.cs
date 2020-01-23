using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectInfoWindow : MonoBehaviour, ISubscriber
{
    private const int SmallWindowCharLimit = 59;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Equipment;
    public TextMeshProUGUI Hp;
    public TextMeshProUGUI Attitude;

    public void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.InspectEntityEventName, this);

        if (gameObject.activeSelf)
        {
            Hide();
        }
    }

    public void Show(Entity entity)
    {
        if (entity == null)
        {
            return;
        }

        if (entity.Fluff != null)
        {
            Name.text = entity.Fluff.Name;

            if (entity.Fluff.FactionName != null)
            {
                Name.text = $"{Name.text}, {entity.Fluff.FactionName}";
            }
        }
        else
        {
            Name.text = entity.EntityType;
        }
        
        Description.text = entity.EntityType; //todo

        if (Description.text.Length > SmallWindowCharLimit && gameObject.name.ToLower().Contains("small"))
        {
            return;
        }

        if (Description.text.Length <= SmallWindowCharLimit && gameObject.name.ToLower().Contains("large"))
        {
            return;
        }


        Equipment.text = GetEquippedItemsForEntity(entity);
        Hp.text = $"{entity.CurrentHp}/{entity.MaxHp}";
        Attitude.text = entity.GetAttitudeTowards(GameManager.Instance.Player).ToString();

        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private static string GetEquippedItemsForEntity(Entity entity)
    {
        return entity.Equipped.Values.Where(item => item != null)
            .Aggregate(string.Empty, (current, item) => current + $", {item.ItemType}");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.InspectEntityEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (GameManager.Instance.AnyActiveWindows() || parameter == null)
        {
            return;
        }

        var entity = parameter as Entity;

        Show(entity);
    }
}
