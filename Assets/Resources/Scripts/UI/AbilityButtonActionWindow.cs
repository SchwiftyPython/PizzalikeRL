﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButtonActionWindow : MonoBehaviour, ISubscriber
{
    private Button _selectedButton;

    public Button AssignButton;
    public Button RemoveButton;

    private void Start()
    {
        EventMediator.Instance.SubscribeToEvent(GlobalHelper.AbilityButtonActionPopupEventName, this);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
    }

    public void OnAssignButtonClick()
    {
        //todo open ability select popup window 
    }

    public void OnRemoveButtonClick()
    {
        _selectedButton.GetComponent<UseAbilityButton>().RemoveAbility();
    }

    public void OnNotify(string eventName, object broadcaster, object parameter = null)
    {
        throw new System.NotImplementedException();
    }

    private void OnDestroy()
    {
        EventMediator.Instance.UnsubscribeFromEvent(GlobalHelper.AbilityButtonActionPopupEventName, this);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }

    private void Show()
    {
        gameObject.SetActive(true);
        GameManager.Instance.AddActiveWindow(gameObject);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.RemoveActiveWindow(gameObject);
    }
}
