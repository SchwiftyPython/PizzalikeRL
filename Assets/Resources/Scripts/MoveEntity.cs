using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEntity {

	Vector2 startTile;
	Vector2 endTile;    

	public bool MoveSuccessful(Entity entity, Vector2 end){
		startTile = entity.currentPosition;
		endTile = end;
        //RaycastHit2D hit = new RaycastHit2D();

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
            Debug.Log("move successful");
            return true;
		} else {
            Debug.Log("move unsuccessful");
            return false;
		}
        
        
        /*
        hit = Physics2D.Raycast(startTile, endTile, 0.9f);
        Debug.Log("Hit: " + hit.collider);

        if (hit.collider == null) {
            entity.currentPosition = endTile;
            entity.SetSpritePosition(endTile);

            //update tile data for start and end tiles
            Tile tileToUpdate = WorldManager.instance.GetTileAt(startTile);
            tileToUpdate.SetBlocksMovement(false);
            tileToUpdate.SetPresentEntity(null);

            tileToUpdate = WorldManager.instance.GetTileAt(endTile);
            tileToUpdate.SetBlocksMovement(true);
            tileToUpdate.SetPresentEntity(entity);
            return true;
        } else {
			return false;
		}
        */
	}
}
