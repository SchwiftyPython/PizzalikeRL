using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenData : MonoBehaviour {

    public string Seed { set; get; }

    public static WorldGenData instance = null;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this) {
            // Destroy the current object, so there is just one 
            Destroy(gameObject);
        }

    }
}
