using System.Collections;
using UnityEngine;

public class EnemyController : AstarAi {
    public static bool ActionTaken; //for basic AI pathfinding testing
    public bool TurnStarted;	
	
	private void Update () {
		if(GameManager.Instance.CurrentState == GameManager.GameState.Enemyturn) {
            if (!TurnStarted) {
                Debug.Log("Enemy turn started");
                TurnStarted = true;
                ActionTaken = false;
                StartCoroutine(MakeDecision());
            }              
        }
	}

    public IEnumerator MakeDecision() {
        var enemy = GameManager.Instance.CurrentAreaPosition.TurnOrder.Peek();
        FindPathToPlayer(enemy.GetSprite(), GameManager.Instance.Player.GetSprite().transform.position); //move toward player test
        yield return new WaitForSeconds(0.1f);
        //Debug.Log(Path);
        //Debug.Log("Present Entity: " + GameManager.Instance.CurrentAreaPosition.GetTileAt(Path.vectorPath[1]).GetPresentEntity());
        if (GameManager.Instance.CurrentAreaPosition.GetTileAt(Path.vectorPath[1]).GetPresentEntity() == null) {
            enemy.Move(Path.vectorPath[1]);
            TurnStarted = false;
            ActionTaken = true;
        }
        else {
            TurnStarted = false;
            ActionTaken = true;
        }
    }
}
