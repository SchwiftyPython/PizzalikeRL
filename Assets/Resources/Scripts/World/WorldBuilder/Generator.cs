using System;
using System.Collections.Generic;
using UnityEngine;
using TinkerWorX.AccidentalNoiseLibrary;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    public string Seed { set; get; }
    private int SeedHashCode { set; get; }
    
    private int _height; 
    private int _width; 

    [Header("Height Map")]
    [SerializeField]
    float _deepWater = 0.05f;
    [SerializeField]
    float _shallowWater = 0.075f;
    [SerializeField]
    float _sand = 0.5f;
    [SerializeField]
    float _grass = 0.7f;
    [SerializeField]
    float _forest = 0.8f;
    [SerializeField]
    float _rock = 0.9f;

    [Header("Heat Map")]
    [SerializeField]
    float _coldestValue = 0.05f;
    [SerializeField]
    float _colderValue = 0.18f;
    [SerializeField]
    float _coldValue = 0.4f;
    [SerializeField]
    float _warmValue = 0.6f;
    [SerializeField]
    float _warmerValue = 0.8f;

    [Header("Moisture Map")]
    [SerializeField]
    float _dryerValue = 0.27f;
    [SerializeField]
    float _dryValue = 0.4f;
    [SerializeField]
    float _wetValue = 0.6f;
    [SerializeField]
    float _wetterValue = 0.8f;
    [SerializeField]
    float _wettestValue = 0.9f;

    [Header("Rivers")]
    [SerializeField]
    int _riverCount = 40;
    [SerializeField]
    float _minRiverHeight = 0.6f;
    [SerializeField]
    int _maxRiverAttempts = 1000;
    [SerializeField]
    int _minRiverTurns = 18;
    [SerializeField]
    int _minRiverLength = 20;
    [SerializeField]
    int _maxRiverIntersections = 2;

    // Noise generator module
    ImplicitFractal _heightMap;
    ImplicitCombiner _heatMap;
    ImplicitFractal _moistureMap;

    // Map data
    MapData _heightData;
    MapData _heatData;
    MapData _moistureData;

    // Final Objects
    Cell[,] _cells;
    private Transform _mapHolder;

    List<CellGroup> _waters = new List<CellGroup>();
    List<CellGroup> _lands = new List<CellGroup>();

    List<River> _rivers = new List<River>();
    List<RiverGroup> _riverGroups = new List<RiverGroup>();

    // Our texture output (unity component)
    MeshRenderer _heightMapRenderer;
    MeshRenderer _heatMapRenderer;
    MeshRenderer _moistureMapRenderer;
    MeshRenderer _biomeMapRenderer;

    private readonly BiomeType[,] _biomeTable = 
    {   
		//COLDEST        //COLDER             //COLD                  //HOT                          //HOTTER                       //HOTTEST
		{ BiomeType.Ice, BiomeType.Wasteland, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
		{ BiomeType.Ice, BiomeType.Wasteland, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
		{ BiomeType.Ice, BiomeType.Wasteland, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Wasteland,           BiomeType.Wasteland },             //DRY
		{ BiomeType.Ice, BiomeType.Swamp,     BiomeType.Wasteland,    BiomeType.Woodland,            BiomeType.Wasteland,           BiomeType.Wasteland },             //WET
		{ BiomeType.Ice, BiomeType.Swamp,     BiomeType.Swamp,        BiomeType.Woodland,            BiomeType.Swamp,               BiomeType.Swamp },              //WETTER
		{ BiomeType.Ice, BiomeType.Swamp,     BiomeType.Swamp,        BiomeType.Swamp,               BiomeType.Swamp,               BiomeType.Swamp }               //WETTEST
    };

    public Capper RarityCapper;

    private void Start()
    {
        Seed = WorldData.Instance.Seed;
        SeedHashCode = Seed.GetHashCode();
        _height = WorldData.Instance.Height;
        _width = WorldData.Instance.Width;

        WorldData.Instance.SaveGameId = Math.Abs(DateTime.Now.GetHashCode()).ToString();

        WorldData.Instance.PopulateToppingsDictionaries();
        
        Initialize();
        Generate();

        WorldData.Instance.Map = _cells;
        
        //todo don't want to start in hostile settlement either
        do
        {
            var x = Random.Range(0, _height);
            var y = Random.Range(0, _width);

            GameManager.Instance.CurrentCell = _cells[x, y];

        } while (GameManager.Instance.CurrentCell.BiomeType == BiomeType.Water ||
                 GameManager.Instance.CurrentCell.BiomeType == BiomeType.Mountain);

        GameManager.Instance.CurrentArea = GameManager.Instance.CurrentCell.Areas[1, 1];
        WorldData.Instance.PlayerStartingPlace = GameManager.Instance.CurrentCell;
        GameManager.Instance.WorldMapGenComplete = true;
        SceneManager.LoadScene("Area");
    }
    #region Public Methods
    public BiomeType GetBiomeType(Cell cell)
    {
        switch (cell.HeightType)
        {
            case HeightType.DeepWater:
            case HeightType.ShallowWater:
                return BiomeType.Water;
            case HeightType.Rock:
                return BiomeType.Mountain;
            default:
                return _biomeTable[(int)cell.MoistureType, (int)cell.HeatType];
        }
    }
    #endregion


    #region Private Methods
    private void Generate()
    {
        Debug.Log("World Generation Started...");
        
        GetData();
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

        CreateFactions();
        PlaceSettlements();

        Debug.Log("Generating History...");

        HistoryGenerator.Instance.Generate();

        Debug.Log("World Generation Complete!");
    }

    private void GenerateBiomeMap()
    {
        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {
                _cells[x, y].biomeType = GetBiomeType(_cells[x, y]);
                _cells[x, y].SetCellSprite();
            }
        }
    }

    private void UpdateBiomeBitmask()
    {
        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {
                _cells[x, y].UpdateBiomeBitmask();
            }
        }
    }

    private void Initialize()
    {
        // Initialize the HeightMap Generator        
        _heightMap = new ImplicitFractal(FractalType.Multi,
            BasisType.Simplex,
            InterpolationType.Quintic,
            SeedHashCode);

        var gradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1);
        var heatFractal = new ImplicitFractal(FractalType.Multi,
            BasisType.Simplex,
            InterpolationType.Quintic,
            SeedHashCode * 3);

        // Combine the gradient with our heat fractal
        _heatMap = new ImplicitCombiner(CombinerType.Multiply);
        _heatMap.AddSource(gradient);
        _heatMap.AddSource(heatFractal);

        _moistureMap = new ImplicitFractal(FractalType.Multi,
            BasisType.Simplex,
            InterpolationType.Quintic,
            SeedHashCode * 6);

    }

    // Extract data from a noise module
    private void GetData()
    {
        _heightData = new MapData(_height, _width);
        _heatData = new MapData(_height, _width);
        _moistureData = new MapData(_height, _width);

        // loop through each x,y point - get height value
        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {

                // WRAP ON BOTH AXIS
                // Noise range
                float x1 = 0, x2 = 2;
                float y1 = 0, y2 = 2;
                var dx = x2 - x1;
                var dy = y2 - y1;

                // Sample noise at smaller intervals
                var s = x / (float)_height;
                var t = y / (float)_width;

                // Calculate our 4D coordinates
                var nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                var ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
                var nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                var nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

                var heightValue = (float)_heightMap.Get(nx, ny, nz, nw);
                var heatValue = (float)_heatMap.Get(nx, ny, nz, nw);
                var moistureValue = (float)_moistureMap.Get(nx, ny, nz, nw);

                // keep track of the max and min values found
                if (heightValue > _heightData.Max) _heightData.Max = heightValue;
                if (heightValue < _heightData.Min) _heightData.Min = heightValue;

                if (heatValue > _heatData.Max) _heatData.Max = heatValue;
                if (heatValue < _heatData.Min) _heatData.Min = heatValue;

                if (moistureValue > _moistureData.Max) _moistureData.Max = moistureValue;
                if (moistureValue < _moistureData.Min) _moistureData.Min = moistureValue;

                _heightData.Data[x, y] = heightValue;
                _heatData.Data[x, y] = heatValue;
                _moistureData.Data[x, y] = moistureValue;
            }
        }

    }

    // Build a Cell array from our data
    private void LoadCells()
    {
        _cells = new Cell[_height, _width];

        for (var currentRow = 0; currentRow < _height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < _width; currentColumn++)
            {
                var c = new Cell
                {
                    X = currentRow,
                    Y = currentColumn,
                    Id = currentRow + " " + currentColumn
                };

                var heightValue = _heightData.Data[currentRow, currentColumn];
                heightValue = (heightValue - _heightData.Min) / (_heightData.Max - _heightData.Min);
                c.HeightValue = heightValue;

                //HeightMap Analyze
                if (heightValue < _deepWater)
                {
                    c.HeightType = HeightType.DeepWater;
                    c.Collidable = false;
                }
                else if (heightValue < _shallowWater)
                {
                    c.HeightType = HeightType.ShallowWater;
                    c.Collidable = false;
                }
                else if (heightValue < _sand)
                {
                    c.HeightType = HeightType.Sand;
                    c.Collidable = true;
                }
                else if (heightValue < _grass)
                {
                    c.HeightType = HeightType.Grass;
                    c.Collidable = true;
                }
                else if (heightValue < _forest)
                {
                    c.HeightType = HeightType.Forest;
                    c.Collidable = true;
                }
                else if (heightValue < _rock)
                {
                    c.HeightType = HeightType.Rock;
                    c.Collidable = true;
                }
                else
                {
                    c.HeightType = HeightType.Snow;
                    c.Collidable = true;
                }

                // Adjust Heat Map based on Height - Higher == colder
                switch (c.HeightType)
                {
                    case HeightType.Forest:
                        _heatData.Data[c.X, c.Y] -= 0.1f * c.HeightValue;
                        break;
                    case HeightType.Rock:
                        _heatData.Data[c.X, c.Y] -= 0.25f * c.HeightValue;
                        break;
                    case HeightType.Snow:
                        _heatData.Data[c.X, c.Y] -= 0.4f * c.HeightValue;
                        break;
                    default:
                        _heatData.Data[c.X, c.Y] += 0.01f * c.HeightValue;
                        break;
                }

                // Set heat value
                var heatValue = _heatData.Data[currentRow, currentColumn];
                heatValue = (heatValue - _heatData.Min) / (_heatData.Max - _heatData.Min);
                c.HeatValue = heatValue;

                // set heat type
                if (heatValue < _coldestValue)
                    c.HeatType = HeatType.Coldest;
                else if (heatValue < _colderValue)
                    c.HeatType = HeatType.Colder;
                else if (heatValue < _coldValue)
                    c.HeatType = HeatType.Cold;
                else if (heatValue < _warmValue)
                    c.HeatType = HeatType.Warm;
                else if (heatValue < _warmerValue)
                    c.HeatType = HeatType.Warmer;
                else
                    c.HeatType = HeatType.Warmest;

                switch (c.HeightType)
                {
                    case HeightType.DeepWater:
                        _moistureData.Data[c.X, c.Y] += 8f * c.HeightValue;
                        break;
                    case HeightType.ShallowWater:
                        _moistureData.Data[c.X, c.Y] += 3f * c.HeightValue;
                        break;
                    case HeightType.Shore:
                        _moistureData.Data[c.X, c.Y] += 1f * c.HeightValue;
                        break;
                    case HeightType.Sand:
                        _moistureData.Data[c.X, c.Y] += 0.2f * c.HeightValue;
                        break;
                }

                //Moisture Map Analyze  
                var moistureValue = _moistureData.Data[currentRow, currentColumn];
                moistureValue = (moistureValue - _moistureData.Min) / (_moistureData.Max - _moistureData.Min);
                c.MoistureValue = moistureValue;

                //set moisture type
                if (moistureValue < _dryerValue) c.MoistureType = MoistureType.Dryest;
                else if (moistureValue < _dryValue) c.MoistureType = MoistureType.Dryer;
                else if (moistureValue < _wetValue) c.MoistureType = MoistureType.Dry;
                else if (moistureValue < _wetterValue) c.MoistureType = MoistureType.Wet;
                else if (moistureValue < _wettestValue) c.MoistureType = MoistureType.Wetter;
                else c.MoistureType = MoistureType.Wettest;

                _cells[currentRow, currentColumn] = c;
            }
        }
    }

    private Cell GetTop(Cell t)
    {
        return _cells[MathHelper.Mod(t.X + 1, _height), t.Y];
    }
    private Cell GetBottom(Cell t)
    {
        return _cells[MathHelper.Mod(t.X - 1, _height), t.Y];
    }
    private Cell GetLeft(Cell t)
    {
        return _cells[t.X, MathHelper.Mod(t.Y - 1, _width)];
    }
    private Cell GetRight(Cell t)
    {
        return _cells[t.X, MathHelper.Mod(t.Y + 1, _width)];
    }

    private void UpdateNeighbors()
    {
        for (var currentRow = 0; currentRow < _height; currentRow++)
        {
            for (var currentColumn = 0; currentColumn < _width; currentColumn++)
            {
                var c = _cells[currentRow, currentColumn];

                c.Top = GetTop(c);
                c.Bottom = GetBottom(c);
                c.Left = GetLeft(c);
                c.Right = GetRight(c);
            }
        }
    }

    private void UpdateBitmasks()
    {
        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {
                _cells[x, y].UpdateBitmask();
            }
        }
    }

    private void FloodFill()
    {
        // Use a stack instead of recursion
        var stack = new Stack<Cell>();

        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {

                var c = _cells[x, y];

                //Tile already flood filled, skip
                if (c.FloodFilled) continue;

                // Land
                if (c.Collidable)
                {
                    var group = new CellGroup();
                    group.Type = CellGroupType.Land;
                    stack.Push(c);

                    while (stack.Count > 0)
                    {
                        FloodFill(stack.Pop(), ref group, ref stack);
                    }

                    if (group.Cells.Count > 0)
                        _lands.Add(group);
                }
                // Water
                else
                {
                    var group = new CellGroup();
                    group.Type = CellGroupType.Water;
                    stack.Push(c);

                    while (stack.Count > 0)
                    {
                        FloodFill(stack.Pop(), ref group, ref stack);
                    }

                    if (group.Cells.Count > 0)
                        _waters.Add(group);
                }
            }
        }
    }

    private void FloodFill(Cell cell, ref CellGroup cells, ref Stack<Cell> stack)
    {
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
        var c = GetTop(cell);
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

    private void GenerateRivers()
    {
        var attempts = 0;
        var rivercount = _riverCount;
        _rivers = new List<River>();

        // Generate some rivers
        while (rivercount > 0 && attempts < _maxRiverAttempts)
        {

            // Get a random tile
            var x = Random.Range(0, _height);
            var y = Random.Range(0, _width);
            var cell = _cells[x, y];

            // validate the tile
            if (!cell.Collidable) continue;
            if (cell.Rivers.Count > 0) continue;

            if (cell.HeightValue > _minRiverHeight)
            {
                // Tile is good to start river from
                var river = new River(rivercount) { CurrentDirection = cell.GetLowestNeighbor() };

                // Figure out the direction this river will try to flow

                // Recursively find a path to water
                FindPathToWater(cell, river.CurrentDirection, ref river);

                // Validate the generated river 
                if (river.TurnCount < _minRiverTurns || river.Cells.Count < _minRiverLength || river.Intersections > _maxRiverIntersections)
                {
                    //Validation failed - remove this river
                    foreach (var c in river.Cells.ToArray())
                    {
                        c.Rivers.Remove(river);
                    }
                }
                else if (river.Cells.Count >= _minRiverLength)
                {
                    //Validation passed - Add river to list
                    _rivers.Add(river);
                    cell.Rivers.Add(river);
                    rivercount--;
                }
            }
            attempts++;
        }
    }

    private void FindPathToWater(Cell cell, Direction direction, ref River river)
    {
        if (cell.Rivers.Contains(river))
            return;

        // check if there is already a river on this tile
        if (cell.Rivers.Count > 0)
            river.Intersections++;

        river.AddCell(cell);

        // get neighbors
        var left = GetLeft(cell);
        var right = GetRight(cell);
        var top = GetTop(cell);
        var bottom = GetBottom(cell);

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
        var min = Mathf.Min(Mathf.Min(Mathf.Min(leftValue, rightValue), topValue), bottomValue);

        // if no minimum found - exit
        if (min == int.MaxValue)
            return;

        //Move to next neighbor
        if (min == leftValue)
        {
            if (left.Collidable)
            {
                if (river.CurrentDirection != Direction.Left)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Left;
                }
                FindPathToWater(left, direction, ref river);
            }
        }
        else if (min == rightValue)
        {
            if (right.Collidable)
            {
                if (river.CurrentDirection != Direction.Right)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Right;
                }
                FindPathToWater(right, direction, ref river);
            }
        }
        else if (min == bottomValue)
        {
            if (bottom.Collidable)
            {
                if (river.CurrentDirection != Direction.Bottom)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Bottom;
                }
                FindPathToWater(bottom, direction, ref river);
            }
        }
        else if (min == topValue)
        {
            if (top.Collidable)
            {
                if (river.CurrentDirection != Direction.Top)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Top;
                }
                FindPathToWater(top, direction, ref river);
            }
        }
    }

    private void BuildRiverGroups()
    {
        //loop each tile, checking if it belongs to multiple rivers
        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {
                var c = _cells[x, y];

                if (c.Rivers.Count > 1)
                {
                    // multiple rivers == intersection
                    RiverGroup group = null;

                    // Does a rivergroup already exist for this group?
                    for (var n = 0; n < c.Rivers.Count; n++)
                    {
                        var tileriver = c.Rivers[n];
                        for (var i = 0; i < _riverGroups.Count; i++)
                        {
                            for (var j = 0; j < _riverGroups[i].Rivers.Count; j++)
                            {
                                var river = _riverGroups[i].Rivers[j];
                                if (river.Id == tileriver.Id)
                                {
                                    group = _riverGroups[i];
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }
                        if (group != null) break;
                    }

                    // existing group found -- add to it
                    if (group != null)
                    {
                        for (var n = 0; n < c.Rivers.Count; n++)
                        {
                            if (!group.Rivers.Contains(c.Rivers[n]))
                                group.Rivers.Add(c.Rivers[n]);
                        }
                    }
                    else   //No existing group found - create a new one
                    {
                        group = new RiverGroup();
                        for (var n = 0; n < c.Rivers.Count; n++)
                        {
                            group.Rivers.Add(c.Rivers[n]);
                        }
                        _riverGroups.Add(group);
                    }
                }
            }
        }
    }

    private void DigRiverGroups()
    {
        for (var i = 0; i < _riverGroups.Count; i++)
        {

            var group = _riverGroups[i];
            River longest = null;

            //Find longest river in this group
            for (var j = 0; j < group.Rivers.Count; j++)
            {
                var river = group.Rivers[j];
                if (longest == null)
                    longest = river;
                else if (longest.Cells.Count < river.Cells.Count)
                    longest = river;
            }

            if (longest != null)
            {
                //Dig out longest path first
                DigRiver(longest);

                for (var j = 0; j < group.Rivers.Count; j++)
                {
                    var river = group.Rivers[j];
                    if (river != longest)
                    {
                        DigRiver(river, longest);
                    }
                }
            }
        }
    }

    // Dig river based on a parent river vein
    private static void DigRiver(River river, River parent)
    {
        var intersectionId = 0;
        var intersectionSize = 0;

        // determine point of intersection
        for (var i = 0; i < river.Cells.Count; i++)
        {
            var t1 = river.Cells[i];
            for (var j = 0; j < parent.Cells.Count; j++)
            {
                var t2 = parent.Cells[j];
                if (t1 == t2)
                {
                    intersectionId = i;
                    intersectionSize = t2.RiverSize;
                }
            }
        }

        var counter = 0;
        var intersectionCount = river.Cells.Count - intersectionId;
        var size = Random.Range(intersectionSize, 5);
        river.Length = river.Cells.Count;

        // randomize size change
        var two = river.Length / 2;
        var three = two / 2;
        var four = three / 2;
        var five = four / 2;

        var twomin = two / 3;
        var threemin = three / 3;
        var fourmin = four / 3;
        var fivemin = five / 3;

        // randomize length of each size
        var count1 = Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        var count2 = count1 + Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        var count3 = count2 + Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        var count4 = count3 + Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            var extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0)
                {
                    count1--; count2--; count3--; count4--; extra--;
                }
                else if (count2 > 0)
                {
                    count2--; count3--; count4--; extra--;
                }
                else if (count3 > 0)
                {
                    count3--; count4--; extra--;
                }
                else if (count4 > 0)
                {
                    count4--; extra--;
                }
            }
        }

        // adjust size of river at intersection point
        switch (intersectionSize) {
            case 1:
                count4 = intersectionCount;
                count1 = 0;
                count2 = 0;
                count3 = 0;
                break;
            case 2:
                count3 = intersectionCount;
                count1 = 0;
                count2 = 0;
                break;
            case 3:
                count2 = intersectionCount;
                count1 = 0;
                break;
            case 4:
                count1 = intersectionCount;
                break;
            default:
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;
                break;
        }

        // dig out the river
        for (var i = river.Cells.Count - 1; i >= 0; i--)
        {

            var c = river.Cells[i];

            if (counter < count1)
            {
                c.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                c.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                c.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                c.DigRiver(river, 1);
            }
            else
            {
                c.DigRiver(river, 0);
            }
            counter++;
        }
    }

    // Dig river
    private void DigRiver(River river)
    {
        var counter = 0;

        // How wide are we digging this river?
        var size = Random.Range(1, 5);
        river.Length = river.Cells.Count;

        // randomize size change
        var two = river.Length / 2;
        var three = two / 2;
        var four = three / 2;
        var five = four / 2;

        var twomin = two / 3;
        var threemin = three / 3;
        var fourmin = four / 3;
        var fivemin = five / 3;

        // randomize lenght of each size
        var count1 = Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        var count2 = count1 + Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        var count3 = count2 + Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        var count4 = count3 + Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            var extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; } else if (count2 > 0) { count2--; count3--; count4--; extra--; } else if (count3 > 0) { count3--; count4--; extra--; } else if (count4 > 0) { count4--; extra--; }
            }
        }

        // Dig it out
        for (var i = river.Cells.Count - 1; i >= 0; i--)
        {
            var c = river.Cells[i];

            if (counter < count1)
            {
                c.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                c.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                c.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                c.DigRiver(river, 1);
            }
            else
            {
                c.DigRiver(river, 0);
            }
            counter++;
        }
    }

    private void AddMoisture(Cell c, float amount)
    {
        _moistureData.Data[c.X, c.Y] += amount;
        c.MoistureValue += amount;
        if (c.MoistureValue > 1)
            c.MoistureValue = 1;

        //set moisture type
        if (c.MoistureValue < _dryerValue) c.MoistureType = MoistureType.Dryest;
        else if (c.MoistureValue < _dryValue) c.MoistureType = MoistureType.Dryer;
        else if (c.MoistureValue < _wetValue) c.MoistureType = MoistureType.Dry;
        else if (c.MoistureValue < _wetterValue) c.MoistureType = MoistureType.Wet;
        else if (c.MoistureValue < _wettestValue) c.MoistureType = MoistureType.Wetter;
        else c.MoistureType = MoistureType.Wettest;
    }

    private void AdjustMoistureMap()
    {
        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {

                var c = _cells[x, y];
                if (c.HeightType == HeightType.River)
                {
                    AddMoisture(c, (int)60);
                }
            }
        }
    }

    private static void CreateFactions()
    {
        if (WorldData.Instance.Factions == null)
        {
            WorldData.Instance.Factions = new Dictionary<string, Faction>();
        }

        FactionTemplateLoader.Initialize();

        var numFactions = Random.Range(4, 7);

        for (var i = 0; i < numFactions; i++)
        {
            var newFaction = new Faction();

            while (WorldData.Instance.Factions.ContainsKey(newFaction.Name))
            {
                newFaction = new Faction();
            }

            WorldData.Instance.Factions.Add(newFaction.Name, newFaction);
        }

//        var factionTypes = FactionTemplateLoader.GetFactionTypes();
//
//        foreach (var factionType in factionTypes)
//        {
//            WorldData.Instance.Factions.Add(factionType, new Faction(FactionTemplateLoader.GetFactionByType(factionType)));
//        }
    }

    private void PlaceSettlements()
    {
        var settlementFloorTiles = WorldData.Instance.SettlementFloorTiles;
        var settlementWallTiles = WorldData.Instance.SettlementWallTiles;

        const float chanceToPlaceCard = 0.005f;

        var deck = new FactionDeck();
        const int numCellsTilNextDraw = 25;

        var currentRow = 0;
        var currentColumn = 0;
        var currentCell = _cells[currentRow, currentColumn];

        foreach (var card in deck.Cards)
        {
            var placed = false;
            while (!placed)
            {
                if (currentCell.BiomeType != BiomeType.Water && currentCell.BiomeType != BiomeType.Mountain)
                {
                    var roll = Random.Range(0.000f, 1.000f);
                    if (roll <= chanceToPlaceCard)
                    {
                        if (currentCell.PresentFactions == null)
                        {
                            currentCell.PresentFactions = new List<Faction>();
                        }
                        currentCell.PresentFactions.Add(card);

                        CreateSettlement(card, currentCell);

                        var index = Random.Range(0, settlementFloorTiles.Length);

                        currentCell.WorldMapSprite.LayerPrefabIndexes[WorldSpriteLayer.SettlementFloor] = index;

                        currentCell.WorldMapSprite.Layers[WorldSpriteLayer.SettlementFloor] = settlementFloorTiles[index];

                        index = Random.Range(0, settlementWallTiles.Length);

                        currentCell.WorldMapSprite.LayerPrefabIndexes[WorldSpriteLayer.SettlementWall] = index;

                        currentCell.WorldMapSprite.Layers[WorldSpriteLayer.SettlementWall] = settlementWallTiles[index];
                        placed = true;
                        //Debug.Log(card + " placed at " + currentX + ", " + currentY);
                    }
                }
                if (currentRow + numCellsTilNextDraw >= _height)
                {
                    currentRow += numCellsTilNextDraw - _height;
                    
                    if (currentColumn + 1 >= _width)
                    {
                        currentColumn = 0;
                    }
                    else
                    {
                        currentColumn ++;
                    }

                }
                else
                {
                    currentRow += numCellsTilNextDraw;
                }
                currentCell = _cells[currentRow, currentColumn];
            }
        }
    }

    private void CreateSettlement(Faction faction, Cell cell)
    {
        var currentPopulation = faction.Population;

        var values = Enum.GetValues(typeof(SettlementSize));

        SettlementSize size;
        int settlementPopulation;

        do
        {
            size = (SettlementSize) values.GetValue(Random.Range(0, values.Length));

            settlementPopulation = currentPopulation - SettlementPrefabStore.SettlementSizePopulationCaps[size];

        } while (currentPopulation < SettlementPrefabStore.SettlementSizePopulationCaps[size]);

        //cell.Settlement = new Settlement(faction, size, cell, settlementPopulation);

        //testing todo change to size appropriate for population
        cell.Settlement = new Settlement(faction, SettlementSize.Hamlet, cell, 10);
    }
    #endregion

}