using UnityEngine;

public class GlobalHelper : MonoBehaviour
{
    public static void DestroyAllChildren(GameObject parent)
    {
        for (var i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }
}
