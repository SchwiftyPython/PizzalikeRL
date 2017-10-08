using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinkerWorX.AccidentalNoiseLibrary;

public class Generator : MonoBehaviour {
    // Adjustable variables for Unity Inspector
    [SerializeField]
    int Width = 512;
    [SerializeField]
    int Height = 512;

    [Header("Height Map")]    
    [SerializeField]
    float DeepWater = 0.2f;
    [SerializeField]
    float ShallowWater = 0.4f;
    [SerializeField]
    float Sand = 0.5f;
    [SerializeField]
    float Grass = 0.7f;
    [SerializeField]
    float Forest = 0.8f;
    [SerializeField]
    float Rock = 0.9f;

    [Header("Heat Map")]   
    [SerializeField]
    float ColdestValue = 0.05f;
    [SerializeField]
    float ColderValue = 0.18f;
    [SerializeField]
    float ColdValue = 0.4f;
    [SerializeField]
    float WarmValue = 0.6f;
    [SerializeField]
    float WarmerValue = 0.8f;

    [Header("Moisture Map")]    
    [SerializeField]
    float DryerValue = 0.27f;
    [SerializeField]
    float DryValue = 0.4f;
    [SerializeField]
    float WetValue = 0.6f;
    [SerializeField]
    float WetterValue = 0.8f;
    [SerializeField]
    float WettestValue = 0.9f;

    [Header("Rivers")]
    [SerializeField]
    int RiverCount = 40;
    [SerializeField]
    float MinRiverHeight = 0.6f;
    [SerializeField]
    int MaxRiverAttempts = 1000;
    [SerializeField]
    int MinRiverTurns = 18;
    [SerializeField]
    int MinRiverLength = 20;
    [SerializeField]
    int MaxRiverIntersections = 2;

    // Noise generator module
    ImplicitFractal HeightMap;
    ImplicitCombiner HeatMap;
    ImplicitFractal MoistureMap;

    // Height map data
    MapData HeightData;
    MapData HeatData;
    MapData MoistureData;

    // Final Objects
    Cell[,] Cells;

    List<CellGroup> Waters = new List<CellGroup>();
    List<CellGroup> Lands = new List<CellGroup>();

    List<River> Rivers = new List<River>();
    List<RiverGroup> RiverGroups = new List<RiverGroup>();

    // Our texture output (unity component)
    MeshRenderer HeightMapRenderer;
    MeshRenderer HeatMapRenderer;
    MeshRenderer MoistureMapRenderer;
    MeshRenderer BiomeMapRenderer;

    BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
		//COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
		{ BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
    };    

    void Start() {
        // Get the mesh we are rendering our output to        
        HeightMapRenderer = GameObject.Find("HeightTexture").GetComponentInChildren<MeshRenderer>();
        HeatMapRenderer = GameObject.Find("HeatTexture").GetComponentInChildren<MeshRenderer>();
        MoistureMapRenderer = GameObject.Find("MoistureTexture").GetComponentInChildren<MeshRenderer>();
        BiomeMapRenderer = GameObject.Find("BiomeTexture").GetComponentInChildren<MeshRenderer>();

        // Initialize the generator
        Initialize();
        Generate();
    }

    void Generate() {        
        // Build the maps
        GetData();
        // Build our final objects based on our data
        LoadCells();

        UpdateNeighbors();

        GenerateRivers();
        BuildRiverGroups();
        DigRiverGroups();
        AdjustMoistureMap();

        UpdateBitmasks();
        FloodFill();

        GenerateBiomeMap();
        UpdateBiomeBitmask();

        // Render a texture representation of our map
        HeightMapRenderer.materials[0].mainTexture = TextureGenerator.GetHeightMapTexture(Width, Height, Cells);
        HeatMapRenderer.materials[0].mainTexture = TextureGenerator.GetHeatMapTexture(Width, Height, Cells);
        MoistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetMoistureMapTexture(Width, Height, Cells);
        BiomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(Width, Height, Cells, ColdestValue, ColderValue, ColdValue);       
    }    

    public BiomeType GetBiomeType(Cell cell) {
        return BiomeTable[(int)cell.MoistureType, (int)cell.HeatType];
    }

    private void GenerateBiomeMap() {
        Debug.Log("Generating Biomes...");
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {

                if (!Cells[x, y].Collidable) continue;

                Cell c = Cells[x, y];
                c.BiomeType = GetBiomeType(c);
            }
        }
    }

    private void UpdateBiomeBitmask() {
        Debug.Log("Updating Biome Bitmasks...");
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                Cells[x, y].UpdateBiomeBitmask();
            }
        }
    }

    private void Initialize() {
        // Initialize the HeightMap Generator
        HeightMap = new ImplicitFractal(FractalType.Multi,
                                        BasisType.Simplex,
                                        InterpolationType.Quintic);

        ImplicitGradient gradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1);
        ImplicitFractal heatFractal = new ImplicitFractal(FractalType.Multi,
                                                          BasisType.Simplex,
                                                          InterpolationType.Quintic);

        // Combine the gradient with our heat fractal
        HeatMap = new ImplicitCombiner(CombinerType.Multiply);
        HeatMap.AddSource(gradient);
        HeatMap.AddSource(heatFractal);

        MoistureMap = new ImplicitFractal(FractalType.Multi,
                                          BasisType.Simplex,
                                          InterpolationType.Quintic);

    }

    // Extract data from a noise module
    private void GetData() {
        HeightData = new MapData(Width, Height);
        HeatData = new MapData(Width, Height);
        MoistureData = new MapData(Width, Height);

        // loop through each x,y point - get height value
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {

                // WRAP ON BOTH AXIS
                // Noise range
                float x1 = 0, x2 = 2;
                float y1 = 0, y2 = 2;
                float dx = x2 - x1;
                float dy = y2 - y1;

                // Sample noise at smaller intervals
                float s = x / (float)Width;
                float t = y / (float)Height;

                // Calculate our 4D coordinates
                float nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                float ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
                float nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                float nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

                float heightValue = (float)HeightMap.Get(nx, ny, nz, nw);
                float heatValue = (float)HeatMap.Get(nx, ny, nz, nw);
                float moistureValue = (float)MoistureMap.Get(nx, ny, nz, nw);

                // keep track of the max and min values found
                if (heightValue > HeightData.Max) HeightData.Max = heightValue;
                if (heightValue < HeightData.Min) HeightData.Min = heightValue;

                if (heatValue > HeatData.Max) HeatData.Max = heatValue;
                if (heatValue < HeatData.Min) HeatData.Min = heatValue;

                if (moistureValue > MoistureData.Max) MoistureData.Max = moistureValue;
                if (moistureValue < MoistureData.Min) MoistureData.Min = moistureValue;

                HeightData.Data[x, y] = heightValue;
                HeatData.Data[x, y] = heatValue;
                MoistureData.Data[x, y] = moistureValue;
            }
        }

    }

    // Build a Cell array from our data
    private void LoadCells() {
        Cells = new Cell[Width, Height];

        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                Cell c = new Cell();
                c.X = x;
                c.Y = y;

                float heightValue = HeightData.Data[x, y];                
                heightValue = (heightValue - HeightData.Min) / (HeightData.Max - HeightData.Min);
                c.HeightValue = heightValue;

                //HeightMap Analyze
                if (heightValue < DeepWater) {
                    c.HeightType = HeightType.DeepWater;
                    c.Collidable = false;
                } else if (heightValue < ShallowWater) {
                    c.HeightType = HeightType.ShallowWater;
                    c.Collidable = false;
                } else if (heightValue < Sand) {
                    c.HeightType = HeightType.Sand;
                    c.Collidable = true;
                } else if (heightValue < Grass) {
                    c.HeightType = HeightType.Grass;
                    c.Collidable = true;
                } else if (heightValue < Forest) {
                    c.HeightType = HeightType.Forest;
                    c.Collidable = true;
                } else if (heightValue < Rock) {
                    c.HeightType = HeightType.Rock;
                    c.Collidable = true;
                } else {
                    c.HeightType = HeightType.Snow;
                    c.Collidable = true;
                }

                // Adjust Heat Map based on Height - Higher == colder
                if (c.HeightType == HeightType.Forest) {
                    HeatData.Data[c.X, c.Y] -= 0.1f * c.HeightValue;
                } else if (c.HeightType == HeightType.Rock) {
                    HeatData.Data[c.X, c.Y] -= 0.25f * c.HeightValue;
                } else if (c.HeightType == HeightType.Snow) {
                    HeatData.Data[c.X, c.Y] -= 0.4f * c.HeightValue;
                } else {
                    HeatData.Data[c.X, c.Y] += 0.01f * c.HeightValue;
                }

                // Set heat value
                float heatValue = HeatData.Data[x, y];
                heatValue = (heatValue - HeatData.Min) / (HeatData.Max - HeatData.Min);
                c.HeatValue = heatValue;

                // set heat type
                if (heatValue < ColdestValue)
                    c.HeatType = HeatType.Coldest;
                else if (heatValue < ColderValue)
                    c.HeatType = HeatType.Colder;
                else if (heatValue < ColdValue)
                    c.HeatType = HeatType.Cold;
                else if (heatValue < WarmValue)
                    c.HeatType = HeatType.Warm;
                else if (heatValue < WarmerValue)
                    c.HeatType = HeatType.Warmer;
                else
                    c.HeatType = HeatType.Warmest;

                if (c.HeightType == HeightType.DeepWater) {
                    MoistureData.Data[c.X, c.Y] += 8f * c.HeightValue;
                } else if (c.HeightType == HeightType.ShallowWater) {
                    MoistureData.Data[c.X, c.Y] += 3f * c.HeightValue;
                } else if (c.HeightType == HeightType.Shore) {
                    MoistureData.Data[c.X, c.Y] += 1f * c.HeightValue;
                } else if (c.HeightType == HeightType.Sand) {
                    MoistureData.Data[c.X, c.Y] += 0.2f * c.HeightValue;
                }

                //Moisture Map Analyze  
                float moistureValue = MoistureData.Data[x, y];
                moistureValue = (moistureValue - MoistureData.Min) / (MoistureData.Max - MoistureData.Min);
                c.MoistureValue = moistureValue;

                //set moisture type
                if (moistureValue < DryerValue) c.MoistureType = MoistureType.Dryest;
                else if (moistureValue < DryValue) c.MoistureType = MoistureType.Dryer;
                else if (moistureValue < WetValue) c.MoistureType = MoistureType.Dry;
                else if (moistureValue < WetterValue) c.MoistureType = MoistureType.Wet;
                else if (moistureValue < WettestValue) c.MoistureType = MoistureType.Wetter;
                else c.MoistureType = MoistureType.Wettest;

                Cells[x, y] = c;
            }
        }
    }

    private Cell GetTop(Cell t) {
        return Cells[t.X, MathHelper.Mod(t.Y - 1, Height)];
    }
    private Cell GetBottom(Cell t) {
        return Cells[t.X, MathHelper.Mod(t.Y + 1, Height)];
    }
    private Cell GetLeft(Cell t) {
        return Cells[MathHelper.Mod(t.X - 1, Width), t.Y];
    }
    private Cell GetRight(Cell t) {
        return Cells[MathHelper.Mod(t.X + 1, Width), t.Y];
    }

    private void UpdateNeighbors() {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                Cell c = Cells[x, y];

                c.Top = GetTop(c);
                c.Bottom = GetBottom(c);
                c.Left = GetLeft(c);
                c.Right = GetRight(c);
            }
        }
    }

    private void UpdateBitmasks() {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                Cells[x, y].UpdateBitmask();
            }
        }
    }

    private void FloodFill() {
        // Use a stack instead of recursion
        Stack<Cell> stack = new Stack<Cell>();

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                Cell c = Cells[x, y];

                //Tile already flood filled, skip
                if (c.FloodFilled) continue;

                // Land
                if (c.Collidable) {
                    CellGroup group = new CellGroup();
                    group.Type = CellGroupType.Land;
                    stack.Push(c);

                    while (stack.Count > 0) {
                        FloodFill(stack.Pop(), ref group, ref stack);
                    }

                    if (group.Cells.Count > 0)
                        Lands.Add(group);
                }
                // Water
                else {
                    CellGroup group = new CellGroup();
                    group.Type = CellGroupType.Water;
                    stack.Push(c);

                    while (stack.Count > 0) {
                        FloodFill(stack.Pop(), ref group, ref stack);
                    }

                    if (group.Cells.Count > 0)
                        Waters.Add(group);
                }
            }
        }
    }

    private void FloodFill(Cell cell, ref CellGroup cells, ref Stack<Cell> stack) {
        // Validate
        if (cell.FloodFilled)
            return;
        if (cells.Type == CellGroupType.Land && !cell.Collidable)
            return;
        if (cells.Type == CellGroupType.Water && cell.Collidable)
            return;

        // Add to TileGroup
        cells.Cells.Add(cell);
        cell.FloodFilled = true;

        // floodfill into neighbors
        Cell c = GetTop(cell);
        if (!c.FloodFilled && cell.Collidable == c.Collidable)
            stack.Push(c);
        c = GetBottom(cell);
        if (!c.FloodFilled && cell.Collidable == c.Collidable)
            stack.Push(c);
        c = GetLeft(cell);
        if (!c.FloodFilled && cell.Collidable == c.Collidable)
            stack.Push(c);
        c = GetRight(cell);
        if (!c.FloodFilled && cell.Collidable == c.Collidable)
            stack.Push(c);
    }

    private void GenerateRivers() {
        int attempts = 0;
        int rivercount = RiverCount;
        Rivers = new List<River>();

        // Generate some rivers
        while (rivercount > 0 && attempts < MaxRiverAttempts) {

            // Get a random tile
            int x = UnityEngine.Random.Range(0, Width);
            int y = UnityEngine.Random.Range(0, Height);
            Cell cell = Cells[x, y];

            // validate the tile
            if (!cell.Collidable) continue;
            if (cell.Rivers.Count > 0) continue;

            if (cell.HeightValue > MinRiverHeight) {
                // Tile is good to start river from
                River river = new River(rivercount);

                // Figure out the direction this river will try to flow
                river.CurrentDirection = cell.GetLowestNeighbor();

                // Recursively find a path to water
                FindPathToWater(cell, river.CurrentDirection, ref river);

                // Validate the generated river 
                if (river.TurnCount < MinRiverTurns || river.Cells.Count < MinRiverLength || river.Intersections > MaxRiverIntersections) {
                    //Validation failed - remove this river
                    for (int i = 0; i < river.Cells.Count; i++) {
                        Cell c = river.Cells[i];
                        c.Rivers.Remove(river);
                    }
                } else if (river.Cells.Count >= MinRiverLength) {
                    //Validation passed - Add river to list
                    Rivers.Add(river);
                    cell.Rivers.Add(river);
                    rivercount--;
                }
            }
            attempts++;
        }
    }

    private void FindPathToWater(Cell cell, Direction direction, ref River river) {
        if (cell.Rivers.Contains(river))
            return;

        // check if there is already a river on this tile
        if (cell.Rivers.Count > 0)
            river.Intersections++;

        river.AddCell(cell);

        // get neighbors
        Cell left = GetLeft(cell);
        Cell right = GetRight(cell);
        Cell top = GetTop(cell);
        Cell bottom = GetBottom(cell);

        float leftValue = int.MaxValue;
        float rightValue = int.MaxValue;
        float topValue = int.MaxValue;
        float bottomValue = int.MaxValue;

        // query height values of neighbors
        if (left.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(left))
            leftValue = left.HeightValue;
        if (right.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(right))
            rightValue = right.HeightValue;
        if (top.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(top))
            topValue = top.HeightValue;
        if (bottom.GetRiverNeighborCount(river) < 2 && !river.Cells.Contains(bottom))
            bottomValue = bottom.HeightValue;

        // if neighbor is existing river that is not this one, flow into it
        if (bottom.Rivers.Count == 0 && !bottom.Collidable)
            bottomValue = 0;
        if (top.Rivers.Count == 0 && !top.Collidable)
            topValue = 0;
        if (left.Rivers.Count == 0 && !left.Collidable)
            leftValue = 0;
        if (right.Rivers.Count == 0 && !right.Collidable)
            rightValue = 0;

        // override flow direction if a tile is significantly lower
        if (direction == Direction.Left)
            if (Mathf.Abs(rightValue - leftValue) < 0.1f)
                rightValue = int.MaxValue;
        if (direction == Direction.Right)
            if (Mathf.Abs(rightValue - leftValue) < 0.1f)
                leftValue = int.MaxValue;
        if (direction == Direction.Top)
            if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                bottomValue = int.MaxValue;
        if (direction == Direction.Bottom)
            if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                topValue = int.MaxValue;

        // find mininum
        float min = Mathf.Min(Mathf.Min(Mathf.Min(leftValue, rightValue), topValue), bottomValue);

        // if no minimum found - exit
        if (min == int.MaxValue)
            return;

        //Move to next neighbor
        if (min == leftValue) {
            if (left.Collidable) {
                if (river.CurrentDirection != Direction.Left) {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Left;
                }
                FindPathToWater(left, direction, ref river);
            }
        } else if (min == rightValue) {
            if (right.Collidable) {
                if (river.CurrentDirection != Direction.Right) {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Right;
                }
                FindPathToWater(right, direction, ref river);
            }
        } else if (min == bottomValue) {
            if (bottom.Collidable) {
                if (river.CurrentDirection != Direction.Bottom) {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Bottom;
                }
                FindPathToWater(bottom, direction, ref river);
            }
        } else if (min == topValue) {
            if (top.Collidable) {
                if (river.CurrentDirection != Direction.Top) {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Top;
                }
                FindPathToWater(top, direction, ref river);
            }
        }
    }

    private void BuildRiverGroups() {
        //loop each tile, checking if it belongs to multiple rivers
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                Cell c = Cells[x, y];

                if (c.Rivers.Count > 1) {
                    // multiple rivers == intersection
                    RiverGroup group = null;

                    // Does a rivergroup already exist for this group?
                    for (int n = 0; n < c.Rivers.Count; n++) {
                        River tileriver = c.Rivers[n];
                        for (int i = 0; i < RiverGroups.Count; i++) {
                            for (int j = 0; j < RiverGroups[i].Rivers.Count; j++) {
                                River river = RiverGroups[i].Rivers[j];
                                if (river.ID == tileriver.ID) {
                                    group = RiverGroups[i];
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }
                        if (group != null) break;
                    }

                    // existing group found -- add to it
                    if (group != null) {
                        for (int n = 0; n < c.Rivers.Count; n++) {
                            if (!group.Rivers.Contains(c.Rivers[n]))
                                group.Rivers.Add(c.Rivers[n]);
                        }
                    } else   //No existing group found - create a new one
                      {
                        group = new RiverGroup();
                        for (int n = 0; n < c.Rivers.Count; n++) {
                            group.Rivers.Add(c.Rivers[n]);
                        }
                        RiverGroups.Add(group);
                    }
                }
            }
        }
    }

    private void DigRiverGroups() {
        for (int i = 0; i < RiverGroups.Count; i++) {

            RiverGroup group = RiverGroups[i];
            River longest = null;

            //Find longest river in this group
            for (int j = 0; j < group.Rivers.Count; j++) {
                River river = group.Rivers[j];
                if (longest == null)
                    longest = river;
                else if (longest.Cells.Count < river.Cells.Count)
                    longest = river;
            }

            if (longest != null) {
                //Dig out longest path first
                DigRiver(longest);

                for (int j = 0; j < group.Rivers.Count; j++) {
                    River river = group.Rivers[j];
                    if (river != longest) {
                        DigRiver(river, longest);
                    }
                }
            }
        }
    }

    // Dig river based on a parent river vein
    private void DigRiver(River river, River parent) {
        int intersectionID = 0;
        int intersectionSize = 0;

        // determine point of intersection
        for (int i = 0; i < river.Cells.Count; i++) {
            Cell t1 = river.Cells[i];
            for (int j = 0; j < parent.Cells.Count; j++) {
                Cell t2 = parent.Cells[j];
                if (t1 == t2) {
                    intersectionID = i;
                    intersectionSize = t2.RiverSize;
                }
            }
        }

        int counter = 0;
        int intersectionCount = river.Cells.Count - intersectionID;
        int size = UnityEngine.Random.Range(intersectionSize, 5);
        river.Length = river.Cells.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize length of each size
        int count1 = UnityEngine.Random.Range(fivemin, five);
        if (size < 4) {
            count1 = 0;
        }
        int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
        if (size < 3) {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + UnityEngine.Random.Range(threemin, three);
        if (size < 2) {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + UnityEngine.Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length) {
            int extra = count4 - river.Length;
            while (extra > 0) {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; } else if (count2 > 0) { count2--; count3--; count4--; extra--; } else if (count3 > 0) { count3--; count4--; extra--; } else if (count4 > 0) { count4--; extra--; }
            }
        }

        // adjust size of river at intersection point
        if (intersectionSize == 1) {
            count4 = intersectionCount;
            count1 = 0;
            count2 = 0;
            count3 = 0;
        } else if (intersectionSize == 2) {
            count3 = intersectionCount;
            count1 = 0;
            count2 = 0;
        } else if (intersectionSize == 3) {
            count2 = intersectionCount;
            count1 = 0;
        } else if (intersectionSize == 4) {
            count1 = intersectionCount;
        } else {
            count1 = 0;
            count2 = 0;
            count3 = 0;
            count4 = 0;
        }

        // dig out the river
        for (int i = river.Cells.Count - 1; i >= 0; i--) {

            Cell c = river.Cells[i];

            if (counter < count1) {
                c.DigRiver(river, 4);
            } else if (counter < count2) {
                c.DigRiver(river, 3);
            } else if (counter < count3) {
                c.DigRiver(river, 2);
            } else if (counter < count4) {
                c.DigRiver(river, 1);
            } else {
                c.DigRiver(river, 0);
            }
            counter++;
        }
    }

    // Dig river
    private void DigRiver(River river) {
        int counter = 0;

        // How wide are we digging this river?
        int size = UnityEngine.Random.Range(1, 5);
        river.Length = river.Cells.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize lenght of each size
        int count1 = UnityEngine.Random.Range(fivemin, five);
        if (size < 4) {
            count1 = 0;
        }
        int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
        if (size < 3) {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + UnityEngine.Random.Range(threemin, three);
        if (size < 2) {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + UnityEngine.Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length) {
            int extra = count4 - river.Length;
            while (extra > 0) {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; } else if (count2 > 0) { count2--; count3--; count4--; extra--; } else if (count3 > 0) { count3--; count4--; extra--; } else if (count4 > 0) { count4--; extra--; }
            }
        }

        // Dig it out
        for (int i = river.Cells.Count - 1; i >= 0; i--) {
            Cell c = river.Cells[i];

            if (counter < count1) {
                c.DigRiver(river, 4);
            } else if (counter < count2) {
                c.DigRiver(river, 3);
            } else if (counter < count3) {
                c.DigRiver(river, 2);
            } else if (counter < count4) {
                c.DigRiver(river, 1);
            } else {
                c.DigRiver(river, 0);
            }
            counter++;
        }
    }

    private void AddMoisture(Cell c, float amount) {
        MoistureData.Data[c.X, c.Y] += amount;
        c.MoistureValue += amount;
        if (c.MoistureValue > 1)
            c.MoistureValue = 1;

        //set moisture type
        if (c.MoistureValue < DryerValue) c.MoistureType = MoistureType.Dryest;
        else if (c.MoistureValue < DryValue) c.MoistureType = MoistureType.Dryer;
        else if (c.MoistureValue < WetValue) c.MoistureType = MoistureType.Dry;
        else if (c.MoistureValue < WetterValue) c.MoistureType = MoistureType.Wet;
        else if (c.MoistureValue < WettestValue) c.MoistureType = MoistureType.Wetter;
        else c.MoistureType = MoistureType.Wettest;
    }

    private void AdjustMoistureMap() {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {

                Cell c = Cells[x, y];
                if (c.HeightType == HeightType.River) {
                    AddMoisture(c, (int)60);
                }
            }
        }
    }

}
