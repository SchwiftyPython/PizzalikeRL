using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectPopup : MonoBehaviour, ISubscriber
{
    public GameObject AvailableAbilityPrefab;
    public GameObject AbilityCategoryPrefab;
    public RectTransform AbilityCategoryParent;
    public GameObject AbilityDescription; //todo make tooltip

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void PopulateAllAbilities()
    {

    }

    private void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        throw new System.NotImplementedException();
    }
}
