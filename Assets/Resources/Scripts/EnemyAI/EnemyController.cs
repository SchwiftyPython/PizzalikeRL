using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AstarAI {
    public bool actionTaken = false; //for basic AI pathfinding testing
    public bool turnStarted = false;	
	
	void Update () {
		if(GameManager.instance.currentState == GameManager.TurnState.ENEMYTURN) {
            if (!turnStarted) {
                Debug.Log("Enemy turn started");
                turnStarted = true;
                StartCoroutine(MakeDecision());
            }              
        }
	}

    public IEnumerator MakeDecision() {
        Entity enemy = WorldManager.instance.enemy;
        FindPathToPlayer(WorldManager.instance.player.GetSprite().transform.position); //move toward player test
        yield return new WaitForSeconds(0.1f);
        Debug.Log(path);
        Debug.Log("Present Entity: " + WorldManager.instance.GetTileAt(path.vectorPath[1]).GetPresentEntity());
        if (WorldManager.instance.GetTileAt(path.vectorPath[1]).GetPresentEntity() == null) {
            enemy.Move(path.vectorPath[1]);
            InputController.instance.actionTaken = false;
            turnStarted = false;
            GameManager.instance.currentState = GameManager.TurnState.PLAYERTURN;
        }else {
            enemy.MeleeAttack(WorldManager.instance.player);
        }
    }
}
