using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WorldManager : MonoBehaviour {

	Random r = new Random();

	public int columns = 50;
	public int rows = 50;
    public float graphOffset = -0.5f;
    public GameObject grass;
	public GameObject wall;
	public GameObject playerSprite;
    public GameObject enemySprite; //for testing
    public GameObject aStar;
    public bool worldSetup = false;
    //public GameObject blockManager;
	public Entity player;
    public Entity enemy; //for testing 
    
	private Transform boardHolder;
	//private List<Vector3> gridPositions = new List<Vector3> (); //store a gridPosition in each tile?
	private Tile[,] board; 

	public static WorldManager instance = null;    

    void Awake () {		
		if (instance == null) {
			instance = this;
		}else if(instance != this){
			// Destroy the current object, so there is just one 
			Destroy(gameObject);
		}        

	}
    	

	/*
	void InitializeList(){
		
		gridPositions.Clear ();

		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				gridPositions.Add (new Vector3 (x, y, 0));
			}
		}
	}
    */

	public void BoardSetup(){
		board = new Tile[columns, rows];
		int maxWalls = columns * (rows / 2);
		int wallCount = 0;        

		boardHolder = new GameObject ("Board").transform;
        
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				GameObject tileTypeToInstantiate;
				bool blocksMovement;
				bool blocksLight;
				int tileType = Random.Range (0, 5);

				if (tileType < 4 || wallCount > maxWalls) {
					tileTypeToInstantiate = grass;
					blocksMovement = false;
					blocksLight = false;
				} else {
					tileTypeToInstantiate = wall;
					blocksMovement = true;
					blocksLight = true;
					wallCount++;
				}

				Tile tile = new Tile (tileTypeToInstantiate, new Vector3 (x, y, 0), blocksMovement, blocksLight);

				GameObject instance = Instantiate (tile.GetTileTexture(), tile.GetGridPosition(), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);

				board [x, y] = tile;
			}
		}        

        PlacePlayer(); 
        PlaceEnemy();   

        //TODO: Make this graph block into a method once functional        
        AstarData data = aStar.GetComponent<AstarPath>().data;
        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;

        gg.width = columns;
        gg.depth = rows;
        gg.nodeSize = 1;
        gg.center = new Vector3(gg.width / 2 + graphOffset, gg.depth / 2 + graphOffset, -0.1f);
        gg.SetDimensions(gg.width, gg.depth, gg.nodeSize);        
        gg.collision.use2D = true;        
        gg.collision.type = ColliderType.Ray;
        gg.collision.mask.value = 256; //Set mask to obstacle        
        gg.rotation.x = -90;
        gg.cutCorners = false;              
        //gg.neighbours = NumNeighbours.Four;        

        AstarPath.active.Scan();
        worldSetup = true;
	}

	void PlacePlayer(){			

		bool placed = false;
		int y = rows / 2;
		int x = columns / 2;
		while (!placed) {
			if (!board [x, y].GetBlocksMovement()) {
				GameObject playerPawn = Instantiate (playerSprite, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                //playerPawn.GetComponent<SingleNodeBlocker>().manager = blockManager.GetComponent<BlockManager>();
				player = new Entity (true, playerPawn);
				board [x, y].SetPresentEntity (player);					
				player.currentPosition = new  Vector3 (x, y, 0f);
				placed = true;
			}
			y++;
			x++;
		}
	}

    void PlaceEnemy(){

        bool placed = false;
        int y = Random.Range(0, rows);
        int x = Random.Range(0, columns);
        while (!placed)
        {
            if (!board[x, y].GetBlocksMovement())
            {
                GameObject enemyPawn = Instantiate(enemySprite, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                //enemyPawn.GetComponent<SingleNodeBlocker>().manager = blockManager.GetComponent<BlockManager>();
                enemy = new Entity(false, enemyPawn);
                board[x, y].SetPresentEntity(enemy);
                enemy.currentPosition = new Vector3(x, y, 0f);
                //enemy.SetBlocker();
                placed = true;
            }
            y = Random.Range(0, rows);
            x = Random.Range(0, columns);
        }

    }

	public Tile GetTileAt(Vector3 position){
		return board [(int)position.x, (int)position.y];
	}

	public void SetTileAt(Vector3 position, Tile tile){
		board [(int)position.x, (int)position.y] = tile; 
	}

}
