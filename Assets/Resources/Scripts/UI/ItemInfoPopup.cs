using TMPro;
using UnityEngine;

public class ItemInfoPopup : MonoBehaviour, ISubscriber
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Modifiers;
    public TextMeshProUGUI Rarity;

    public void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.InspectItemEventName, this);

        if (gameObject.activeSelf)
        {
            Hide();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void Show(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item.ItemCategory.Equals("weapon"))
        {
            Name.text =
                $"{ItemStore.Instance.GetDisplayNameForItemType(item.ItemType)}     [ {item.ItemDice.NumDice}d{item.ItemDice.NumSides} ]"; //todo add a sword icon
        }
        else if (item.ItemCategory.Equals("armor"))
        {
            var defense = ((Armor) item).Defense;
            Name.text =
                $"{ItemStore.Instance.GetDisplayNameForItemType(item.ItemType)}     [ {defense} def ]"; //todo replace def with a shield icon
        }

        Description.text = string.Empty;

        Modifiers.text = string.Empty;

        Rarity.text = item.Rarity.ToString();

        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.InspectItemEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        if (!eventName.Equals(GlobalHelper.InspectItemEventName) || parameter == null)
        {
            return;
        }

        if (!(parameter is Item item))
        {
            return;
        }

        Show(item);
    }
}
