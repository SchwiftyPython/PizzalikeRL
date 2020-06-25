using UnityEngine;

public class SituationLoader : MonoBehaviour
{
    public TextAsset StartSituationFile;
    public TextAsset MiddleSituationFile;
    public TextAsset EndSituationFile;

    public static SituationLoader Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
