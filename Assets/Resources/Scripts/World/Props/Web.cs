using UnityEngine;

public class Web : MonoBehaviour
{
    private readonly GameObject _prefab = WorldData.Instance.SpiderWebPrefab;

    public Web(Tile tile)
    {
        tile.AbilityTexture = Instantiate(_prefab,
            new Vector2(tile.Y, tile.X), Quaternion.identity);
    }

    public Web() {}
}
