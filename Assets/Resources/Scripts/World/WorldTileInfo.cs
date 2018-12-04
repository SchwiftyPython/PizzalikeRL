using UnityEngine;
using UnityEngine.UI;

public class WorldTileInfo : MonoBehaviour
{
    public GameObject Window;
    public Text SettlementName;

    private void Awake()
    {
        Window = WorldMap.Instance.CellInfoWindow;
        SettlementName = Window.GetComponentInChildren<Text>();
        Window.SetActive(false);
    }

    public void OnRightClick()
    {
        var position = GetComponent<Transform>().position;
        var clickedCell = WorldData.Instance.Map[(int)position.x, (int)position.y];

        Show(clickedCell);
    }

    public void Show(Cell clickedCell)
    {
        SettlementName.text = "Settlement Name: " + clickedCell.Settlement.Name;

        Window.SetActive(true);
    }

    public void Hide()
    {
        Window.SetActive(false);
    }
}
