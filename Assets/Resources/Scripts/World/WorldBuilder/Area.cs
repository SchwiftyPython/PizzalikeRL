using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area {
    private const int Height = 32;
    private const int Width = 32;

    public BiomeType biomeType { get; set; }

    Tile[,] Tiles =  new Tile[Height, Width];

    public Area() {
        
    }
}
