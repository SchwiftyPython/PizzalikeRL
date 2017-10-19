using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area {
    int height = 64;
    int width = 64;

    Tile[,] Tiles;

    public Area() {
        Tiles = new Tile[height, width];
    }
}
