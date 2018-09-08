using UnityEngine;
using UnityEngine.UI;

public class ActionWindow : MonoBehaviour
{
    private Entity _player;
    private Tile _selectedTile;

    public GameObject Window;

    public GameObject MoveHereButton;
    public GameObject RangedAttackButton;
    public GameObject MeleeAttackButton;

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
        _selectedTile = tile;
        _player = GameManager.Instance.Player;

        MoveHereButton.GetComponent<Button>().interactable = !tile.GetBlocksMovement();

        if (tile.GetPresentEntity() != null && _player.HasRangedWeaponEquipped() &&
            _player.EquippedWeaponInRangeOfTarget(tile.GetPresentEntity()))
        {
            RangedAttackButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            RangedAttackButton.GetComponent<Button>().interactable = false;
        }

        if (tile.GetPresentEntity() != null &&
            _player.CalculateDistanceToTarget(tile.GetPresentEntity()) < 2)
        {
            MeleeAttackButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            MeleeAttackButton.GetComponent<Button>().interactable = false;
        }

        var pos = Input.mousePosition;

        Window.transform.position = new Vector2(pos.x + 60f, pos.y + 50f);

        Window.SetActive(true);
    }

    public void OnMoveHereButtonClicked()
    {
        //todo auto move
        AfterActionCleanup();
    }

    public void OnRangedAttackButtonClicked()
    {
        _player.RangedAttack(_selectedTile.GetPresentEntity());
        AfterActionCleanup();
    }

    public void OnMeleeAttackButtonClicked()
    {
        _player.MeleeAttack(_selectedTile.GetPresentEntity());
        AfterActionCleanup();
    }

    private void AfterActionCleanup()
    {
        InputController.Instance.ClearHighlights();
        Window.SetActive(false);
        GameManager.Instance.CurrentState = GameManager.GameState.EndTurn;
    }
}
