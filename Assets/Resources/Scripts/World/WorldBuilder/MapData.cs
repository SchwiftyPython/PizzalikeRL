using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData { 

    public float[,] Data;
    public float Min { get; set; }
    public float Max { get; set; }

    public MapData(int height, int width) {
        Data = new float[height, width];
        Min = float.MaxValue;
        Max = float.MinValue;
    }
}
