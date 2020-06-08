using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Grave : Prop
{
    [Serializable]
    public struct Inscription
    {
        public string DeceasedName;
        public string DateOfBirth;
        public string DateOfDeath;
        public string CauseOfDeath;
        public string Epitaph;
    }

    public Guid Id;
    public Inscription inscription;

    public Grave(string prefabKey, GameObject prefab, string name, string dateOfDeath, string dateOfBirth, string causeOfDeath) : base(prefabKey, prefab)
    {
        Id = Guid.NewGuid();

        inscription.DeceasedName = name;
        inscription.DateOfBirth = dateOfBirth;
        inscription.DateOfDeath = dateOfDeath;
        inscription.CauseOfDeath = causeOfDeath;

        //todo epitaph

        WorldData.Instance.Graves.Add(Id, this);
    }

    public Grave(string prefabKey, GameObject prefab) : base(prefabKey, prefab)
    {
        Id = Guid.NewGuid();

        GenerateInscription();

        //todo epitaph

        WorldData.Instance.Graves.Add(Id, this);
    }

    public Grave(GraveSdo sdo)
    {
        Id = sdo.Id;
        inscription = sdo.Inscription;

        var tiles = WorldData.Instance.GraveyardProps;

        Prefab = tiles[Random.Range(0, tiles.Length)];
        IsContainer = false;

        if (!WorldData.Instance.Graves.ContainsKey(Id))
        {
            WorldData.Instance.Graves.Add(Id, this);
        }
    }

    private void GenerateInscription()
    {
        inscription.DeceasedName = NameStore.Instance.GenerateFullName();
        //        inscription.dateOfBirth =
        //        inscription.dateOfDeath = 
        //        inscription.causeOfDeath = 
    }
}
