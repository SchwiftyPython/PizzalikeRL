using UnityEngine;
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    public Vector2 TargetPosition;
    public Path Path;
    public bool PathCalculated;

    Seeker _seeker;

    private void Start()
    {
        _seeker = GetComponent<Seeker>();
        //Debug.Log("seeker: " + _seeker);
    }

    public void FindPathToTarget(Entity currentEntity, Vector2 target)
    {
        PathCalculated = false;
        //Debug.Log("Target: " + target);
        //Debug.Log("seeker: " + _seeker);
        //_seeker = GetComponent<Seeker>();
        //Debug.Log("enemy:" + currentEntity.transform.position);
        _seeker.StartPath(currentEntity.CurrentPosition, target, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("Path returned. Error? " + p.error);
        if (!p.error)
        {
            Path = p;
            PathCalculated = true;
            //Debug.Log("Vector Path: " + Path.vectorPath[1]);
        }
    }
}
