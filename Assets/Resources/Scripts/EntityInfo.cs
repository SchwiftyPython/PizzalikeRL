using UnityEngine;
using UnityEngine.UI;

public class EntityInfo : MonoBehaviour
{
    public GameObject Window;
    public Text EntityName;
    public Text Stats;

    private void Init()
    {
        Window = AreaMap.Instance.ObjectInfoWindow;
        var textBoxes = Window.GetComponentsInChildren<Text>();
        EntityName = textBoxes[0];
        Stats = textBoxes[1];
    }

    public void OnRightClick()
    {
        if (Window == null)
        {
            Init();
        }
        var position = GetComponent<Transform>().position;
        var clickedEntity = GameManager.Instance.CurrentArea.AreaTiles[(int)position.y, (int)position.x].GetPresentEntity();

//        var entityName = clickedEntity.GetTypeForEntityInfoWindow();
//        var entityStats = clickedEntity.GetStatsForEntityInfoWindow();

        Show(clickedEntity);
    }

    public void Show(Entity clickedEntity)
    {
        EntityName.text = clickedEntity.Fluff != null
            ? clickedEntity.GetFluffForEntityInfoWindow()
            : clickedEntity.GetTypeForEntityInfoWindow();

        Stats.text = clickedEntity.GetStatsForEntityInfoWindow();

        //todo provide option to see background info if available

        //todo have window popup near clicked entity
        //Need some logic to make sure window isn't cutoff by screen
        //Window.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 50); 

        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }

}
