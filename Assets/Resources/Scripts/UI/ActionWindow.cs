using UnityEngine;
using UnityEngine.UI;

public class ActionWindow : MonoBehaviour
{
    public GameObject Window;

    public GameObject MoveHereButton;
    public GameObject RangedAttackButton;
    public GameObject MeleeAttackButton;

    public Tile SelectedTile;

    public static ActionWindow Instance;
    
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        Window.SetActive(false);
    }

    public void OnTileSelected(Tile tile)
    {
        var player = GameManager.Instance.Player;

        MoveHereButton.GetComponent<Button>().interactable = !tile.GetBlocksMovement();

        if (tile.GetPresentEntity() != null && player.HasRangedWeaponEquipped() &&
            player.EquippedWeaponInRangeOfTarget(tile.GetPresentEntity()))
        {
            RangedAttackButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            RangedAttackButton.GetComponent<Button>().interactable = false;
        }

        if (tile.GetPresentEntity() != null &&
            player.CalculateDistanceToTarget(tile.GetPresentEntity()) < 2)
        {
            MeleeAttackButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            MeleeAttackButton.GetComponent<Button>().interactable = false;
        }

        var selectedTilePosition = tile.GetGridPosition();

        var pos = Input.mousePosition;

        Window.transform.position = new Vector2(pos.x + 60f, pos.y + 50f);

        Window.SetActive(true);

    }

    public void OnMoveHereButtonClicked()
    {
        Window.SetActive(false);
    }

    public void OnRangedAttackButtonClicked()
    {
        Window.SetActive(false);
    }

    public void OnMeleeAttackButtonClicked()
    {
        Window.SetActive(false);
    }
}
