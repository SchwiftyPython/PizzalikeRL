using System.Collections;
using UnityEngine;

public class EnemyController : AstarAI {
    public static bool ActionTaken; //for basic AI pathfinding testing
    public bool TurnStarted;	
	
	private void Update () {
		if(GameManager.Instance.CurrentState == GameManager.GameState.Enemyturn) {
            if (!TurnStarted) {
                //Debug.Log("Enemy turn started");
                TurnStarted = true;
                ActionTaken = false;
                StartCoroutine(MakeDecision());
            }              
        }
	}

    public IEnumerator MakeDecision()
    {
        var enemy = GameManager.Instance.CurrentArea.TurnOrder.Peek();
        FindPathToTarget(enemy, GameManager.Instance.Player.CurrentPosition); //move toward player test
        yield return new WaitForSeconds(0.1f);
        //Debug.Log(Path);
        //Debug.Log("Present Entity: " + GameManager.Instance.CurrentArea.GetTileAt(Path.vectorPath[1]).GetPresentEntity());
        if (GameManager.Instance.CurrentArea.GetTileAt(Path.vectorPath[1]).GetPresentEntity() == null)
        {
            var nextTilePosition = new Vector2((int)Path.vectorPath[1].x, (int)Path.vectorPath[1].y);
            enemy.AreaMove(nextTilePosition);
            TurnStarted = false;
            ActionTaken = true;
        }
        else
        {
            TurnStarted = false;
            ActionTaken = true;
        }
    }
}
