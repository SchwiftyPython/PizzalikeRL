using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area {
    private const int Height = 32;
    private const int Width = 32;

    public BiomeType BiomeType { get; set; }

    private Tile[,] _areaTiles =  new Tile[Height, Width];

    public Area() {
        
    }
}
