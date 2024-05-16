using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileMod : MonoBehaviour
{
    [HideInInspector] public Vector3 NextSpawnPos;
    [HideInInspector] public Quaternion NextSpawnRot;
    public static int MaxNumObstacleOnOneBlock = 2;
    public GameObject PreventGoingBackCollider;

    public Vector3 Direction
    {
        get { return _direction; }
    }
    protected Vector3 _direction;

    [Header("Offsets")]
    public Vector3 NextBlockOffset;

    public WayPoint[] Waypoints;

    /// Obstacles related
    protected Obstacles[] _obstacles;
    protected int[] _activeObstacleIndex;
    protected int _numActiveObstacles = 0;
    [Tooltip("Obstacle Probability")]
    public int NoSpawnProbability = 90;

    public GameObject Goal;
    public Transform GoalSpawnPos;

    public virtual void OnEnable()
    {
        if (_obstacles == null || _obstacles.Length == 0)
        {
            _obstacles = GetComponentsInChildren<Obstacles>();
            foreach (Obstacles ob in _obstacles)
            {
                ob.Initialization();
            }
            _activeObstacleIndex = new int[MaxNumObstacleOnOneBlock];
            for (int i = 0; i < MaxNumObstacleOnOneBlock; i++)
            {
                _activeObstacleIndex[i] = -1;
            }
            _numActiveObstacles = 0;
        }
        if (!Loader.LoaderInitialized)
        {
            FindObjectOfType<Loader>().Initialization();
        }
        /// Make sure no obstacles are spawned on the first two tiles
        if (TileManager.Instance._indexToSpawn < TileManager.Instance.TotalTileNum - 1 && TileManager.Instance._indexToSpawn > 1)
        {
            RandomlyActivateObstacles();
        }
        else if (TileManager.Instance._indexToSpawn == TileManager.Instance.TotalTileNum - 1)
        {
            SpawnGoal();
        }
    }

    private void OnDisable()
    {
        if (PreventGoingBackCollider != null) PreventGoingBackCollider.SetActive(false);
    }

    protected virtual void CalculateNextTilePosRot()
    {

    }

    protected void SpawnGoal()
    {
        if (Goal != null && GoalSpawnPos != null)
        {
            Transform goal = Instantiate(Goal, GoalSpawnPos).transform;
            goal.localPosition = Vector3.zero;
            goal.localRotation = Quaternion.identity;
        }
    }

    public virtual Vector3 CalculateNextTilePos(Vector3 prevPos, Vector3 WorldDir, CurvedDirection curveDir)
    {
        return Vector3.zero;
    }

    public virtual Quaternion CalculateNextTileRot(Vector3 WorldDir)
    {
        return Quaternion.LookRotation(WorldDir);
    }

    public virtual void ResetPosition(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        GetComponentInChildren<OnTriggerSpawnNewTiles>().bNextTileSpawned = false;
    }

    protected virtual void RandomlyActivateObstacles()
    {
        if (_obstacles.Length == 0 || !TileManager.Instance.bSpawnObstacles) return;
        /// disable old obstacles
        for (int i=0; i<_numActiveObstacles; i++)
        {
            _obstacles[_activeObstacleIndex[i]].DeactivateObstacle();
            _activeObstacleIndex[i] = -1;
        }
        _numActiveObstacles = 0;

        /// randomly select new obstacles to activate
        for (int i=0; i<_obstacles.Length; i++)
        {
            if (_numActiveObstacles == MaxNumObstacleOnOneBlock) break;
            int randomIndex = Random.Range(0, _obstacles.Length - 1);
            while (_obstacles[randomIndex].ActiveObsIndex != -1)
            {
                randomIndex++;
                if (randomIndex == _obstacles.Length) randomIndex = 0;
            }
            int randomVal = Random.Range(1, 100);
            if (randomVal >= NoSpawnProbability)
            {
                _obstacles[randomIndex].ActivateARandomObstacle();
                _activeObstacleIndex[_numActiveObstacles++] = randomIndex;
            }
        }
        
    }
}
