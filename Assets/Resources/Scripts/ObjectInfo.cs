using UnityEngine;
using UnityEngine.UI;

public class ObjectInfo : MonoBehaviour
{
    public GameObject Window;
    public Text ObjectName;
    public Text ObjectDescription;

    private void Init()
    {

        Window = AreaMap.Instance.ObjectInfoWindow;
        var textBoxes = Window.GetComponentsInChildren<Text>();
        ObjectName = textBoxes[0];
        ObjectDescription = textBoxes[1];
    }

    public void OnLeftClick()
    {
        if (Window == null)
        {
            Init();
        }
        var position = GetComponent<Transform>().position;
        var clickedEntity = GameManager.Instance.CurrentArea.AreaTiles[(int)position.x, (int)position.y].GetPresentEntity();
        Show(clickedEntity.EntityType, "walla walla bing bang");
    }

    public void Show(string objectName, string objectDescription)
    {
        ObjectName.text = name;
        ObjectDescription.text = objectDescription;
        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

}
