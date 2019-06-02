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

    public static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
