using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DelivererSelectWindow : MonoBehaviour
{
    private IDictionary<Guid, Entity> _descendants;

    private const int MinDescendants = 2;
    private const int MaxDescendants = 4;

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
        var numDescendants = Random.Range(MinDescendants, MaxDescendants + 1);

        for (var i = 0; i < numDescendants; i++)
        {
            var descendant = new Entity(GameManager.Instance.Player, null, true);

            _descendants.Add(descendant.Id, descendant);

            var descendantButton = Instantiate(DescendantPrefab, new Vector3(0, 0), Quaternion.identity);
            descendantButton.transform.SetParent(DescendantButtonParent.transform);

            var descendantTitle = descendantButton.GetComponentInChildren<Text>();
            descendantTitle.text = $"{descendant.Fluff.Name}, {descendant.Fluff.BackgroundType}";

            var playerSprite = descendantButton.GetComponentsInChildren<Image>()[1];
            playerSprite.sprite = descendant.GetSpritePrefab().GetComponent<SpriteRenderer>().sprite;
        }
    }
}
