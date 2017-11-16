using UnityEngine;
using Pathfinding;

public class WorldManager : MonoBehaviour {

	public int Columns = 50;
	public int Rows = 50;
    public float GraphOffset = -0.5f;
    public GameObject Grass;
	public GameObject Wall;
	public GameObject PlayerSprite;
    public GameObject EnemySprite; //for testing
    public GameObject AStar;
    public bool WorldSetup;
    //public GameObject blockManager;
	public Entity Player;
    public Entity Enemy; //for testing 
    
	private Transform _boardHolder;
	//private List<Vector3> gridPositions = new List<Vector3> (); //store a gridPosition in each tile?
	private Tile[,] _board; 

	public static WorldManager Instance;    

    void Awake () {		
		if (Instance == null) {
			Instance = this;
		}else if(Instance != this){
			// Destroy the current object, so there is just one 
			Destroy(gameObject);
		}        

	}

	public void BoardSetup(){
		_board = new Tile[Columns, Rows];
		var maxWalls = Columns * (Rows / 2);
		var wallCount = 0;        

		_boardHolder = new GameObject ("Board").transform;
        
		for (var x = 0; x < Columns; x++) {
			for (var y = 0; y < Rows; y++) {
				GameObject tileTypeToInstantiate;
				bool blocksMovement;
				bool blocksLight;
				var tileType = Random.Range (0, 5);

				if (tileType < 4 || wallCount > maxWalls) {
					tileTypeToInstantiate = Grass;
					blocksMovement = false;
					blocksLight = false;
				} else {
					tileTypeToInstantiate = Wall;
					blocksMovement = true;
					blocksLight = true;
					wallCount++;
				}

				var tile = new Tile (tileTypeToInstantiate, new Vector3 (x, y, 0), blocksMovement, blocksLight);

				var instance = Instantiate (tile.GetTileTexture(), tile.GetGridPosition(), Quaternion.identity);
				instance.transform.SetParent (_boardHolder);

				_board [x, y] = tile;
			}
		}        

        PlacePlayer(); 
        PlaceEnemy();   

        //TODO: Make this graph block into a method once functional        
        var data = AStar.GetComponent<AstarPath>().data;
        var gg = data.AddGraph(typeof(GridGraph)) as GridGraph;

	    if (gg != null)
	    {
	        gg.width = Columns;
	        gg.depth = Rows;
	        gg.nodeSize = 1;
	        gg.center = new Vector3(gg.width / 2 + GraphOffset, gg.depth / 2 + GraphOffset, -0.1f);
	        gg.SetDimensions(gg.width, gg.depth, gg.nodeSize);
	        gg.collision.use2D = true;
	        gg.collision.type = ColliderType.Ray;
	        gg.collision.mask.value = 256; //Set mask to obstacle        
	        gg.rotation.x = -90;
	        gg.cutCorners = false;
	    }
	    //gg.neighbours = NumNeighbours.Four;        

        AstarPath.active.Scan();
        WorldSetup = true;
	}

	void PlacePlayer(){			

		var placed = false;
		var y = Rows / 2;
		var x = Columns / 2;
		while (!placed) {
			if (!_board [x, y].GetBlocksMovement()) {
				var playerPawn = Instantiate (PlayerSprite, new Vector3(x, y, 0f), Quaternion.identity);
                //playerPawn.GetComponent<SingleNodeBlocker>().manager = blockManager.GetComponent<BlockManager>();
				Player = new Entity (true, playerPawn);
				_board [x, y].SetPresentEntity (Player);					
				Player.CurrentPosition = new  Vector3 (x, y, 0f);
				placed = true;
			}
			y++;
			x++;
		}
	}

    void PlaceEnemy(){

        var placed = false;
        var y = Random.Range(0, Rows);
        var x = Random.Range(0, Columns);
        while (!placed)
        {
            if (!_board[x, y].GetBlocksMovement())
            {
                var enemyPawn = Instantiate(EnemySprite, new Vector3(x, y, 0f), Quaternion.identity);
                //enemyPawn.GetComponent<SingleNodeBlocker>().manager = blockManager.GetComponent<BlockManager>();
                Enemy = new Entity(false, enemyPawn);
                _board[x, y].SetPresentEntity(Enemy);
                Enemy.CurrentPosition = new Vector3(x, y, 0f);
                //enemy.SetBlocker();
                placed = true;
            }
            y = Random.Range(0, Rows);
            x = Random.Range(0, Columns);
        }

    }

    public void RemoveDeadEntity(Entity corpse) {
        if (corpse.IsPlayer()) {
            GameManager.Instance.CurrentState = GameManager.GameState.End;
        } else {
            //remove entity and replace with some or all of inventory
            var tileToUpdate = Instance.GetTileAt(corpse.CurrentPosition);
            tileToUpdate.SetBlocksMovement(false);
            tileToUpdate.SetPresentEntity(null);

            corpse.SetSprite(null);
        }
    }

	public Tile GetTileAt(Vector3 position){
		return _board [(int)position.x, (int)position.y];
	}

	public void SetTileAt(Vector3 position, Tile tile){
		_board [(int)position.x, (int)position.y] = tile; 
	}

}
