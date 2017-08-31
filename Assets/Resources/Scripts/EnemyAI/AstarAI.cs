using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AstarAI : MonoBehaviour {
    public Vector2 targetPosition;         
    public Path path;
    public bool pathCalculated;

    Seeker seeker;

    void Start () {
        seeker = GetComponent<Seeker>();        
	}

    public void FindPathToPlayer(Vector2 target) {
        pathCalculated = false;
        Debug.Log("Target: " + target);
        GameObject enemy = WorldManager.instance.enemy.GetSprite();        
        seeker = GetComponent<Seeker>();
        Debug.Log("enemy:" + enemy.transform.position);
        seeker.StartPath(enemy.transform.position, target, OnPathComplete);
        
    }
	
    public void OnPathComplete(Path p) {
        Debug.Log("Path returned. Error? " + p.error);
        if (!p.error) {
            path = p;
            pathCalculated = true;
            Debug.Log("Vector Path: " + path.vectorPath[1]);
        }
    }
}
