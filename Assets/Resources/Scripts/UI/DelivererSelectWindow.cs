using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DelivererSelectWindow : MonoBehaviour
{
    private IDictionary<Guid, Entity> _descendants;

    public GameObject DescendantPrefab;
    public GameObject DescendantButtonParent;
    public GameObject DescendantDescription;

    public GameObject StrengthValueBox;
    public GameObject AgilityValueBox;
    public GameObject ConstitutionValueBox;
    public GameObject IntelligenceValueBox;

    public static DelivererSelectWindow Instance;

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
    }

    private void Start()
    {
        _descendants = new Dictionary<Guid, Entity>();
        PopulateWindow();
    }

    private void PopulateWindow()
    {
        var numDescendants = Random.Range(2, 5);

        for (var i = 0; i < numDescendants; i++)
        {
            var descendant = new Entity(GameManager.Instance.Player, null, true);

            _descendants.Add(descendant.Id, descendant);
        }
    }
}
