using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grave : Prop
{
    public struct Inscription
    {
        public string deceasedName;
        public string dateOfBirth;
        public string dateOfDeath;
        public string causeOfDeath;
        public string epitaph;
    }

    public Guid Id;
    public Inscription inscription;

    public Grave(GameObject prefab, string name, string dateOfDeath, string dateOfBirth, string causeOfDeath) : base(prefab)
    {
        Id = Guid.NewGuid();

        inscription.deceasedName = name;
        inscription.dateOfBirth = dateOfBirth;
        inscription.dateOfDeath = dateOfDeath;
        inscription.causeOfDeath = causeOfDeath;

        //todo epitaph

        WorldData.Instance.Graves.Add(Id, this);
    }

    public Grave(GameObject prefab) : base(prefab)
    {
        Id = Guid.NewGuid();

        GenerateInscription();

        //todo epitaph

        WorldData.Instance.Graves.Add(Id, this);
    }

    private void GenerateInscription()
    {
        inscription.deceasedName = NameStore.Instance.GenerateFullName();
        //        inscription.dateOfBirth =
        //        inscription.dateOfDeath = 
        //        inscription.causeOfDeath = 
    }
}
