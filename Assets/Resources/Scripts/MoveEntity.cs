using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEntity {

	protected Vector2 startTile;
	protected Vector2 endTile;    

    /*
	public virtual bool MoveSuccessful(Vector2 end){
        return true;

        
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
        
	}
    */

    public virtual void Move(Vector2 target) {}

    public virtual bool TargetTileBlocked(Vector2 target) {
        return WorldManager.Instance.GetTileAt(target).GetBlocksMovement();
    }

    public virtual bool TargetTileBlockedByEntity(Vector2 target) {
        if(WorldManager.Instance.GetTileAt(target).GetPresentEntity() == null) {
            return false;
        } else {
            return true;
        }
    }
}
