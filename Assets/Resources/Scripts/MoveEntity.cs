using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEntity {

	static Vector3 startTile;
	static Vector3 endTile;

	public static bool MoveSuccessful(Entity entity, Vector3 end){
		startTile = entity.currentPosition;
		endTile = end;

		Debug.Log ("entity.currentPosition: " + entity.currentPosition.x + " " + entity.currentPosition.y);
		Debug.Log ("start: " + startTile);
		Debug.Log ("End: " + endTile);

		if (!WorldManager.instance.GetTileAt (endTile).GetBlocksMovement ()) {
			entity.currentPosition = endTile;
			entity.SetSpritePosition(endTile);

			//update tile data for start and end tiles
			Tile tileToUpdate = WorldManager.instance.GetTileAt(startTile);
			tileToUpdate.SetBlocksMovement (false);
			tileToUpdate.SetPresentEntity (null);

			tileToUpdate =  WorldManager.instance.GetTileAt(endTile);
			tileToUpdate.SetBlocksMovement (true);
			tileToUpdate.SetPresentEntity (entity);
			return true;
		} else {
			return false;
		}
	}
}
