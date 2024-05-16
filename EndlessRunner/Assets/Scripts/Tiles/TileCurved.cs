using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCurved : TileMod
{
    public float _tileLength;
    public static int MaxNumObstacleOnCurvedBlock = 3;
    public CurvedDirection _curvedDir;

    public override void OnEnable()
    {
        if (_obstacles == null || _obstacles.Length == 0)
        {
            _obstacles = GetComponentsInChildren<Obstacles>();
            foreach (Obstacles ob in _obstacles)
            {
                ob.Initialization();
            }
            _activeObstacleIndex = new int[MaxNumObstacleOnCurvedBlock];
            for(int i=0; i<MaxNumObstacleOnCurvedBlock; i++)
            {
                _activeObstacleIndex[i] = -1;
            }
            _numActiveObstacles = 0;
        }
        if (!Loader.LoaderInitialized)
        {
            FindObjectOfType<Loader>().Initialization();
        }
        if (TileManager.Instance._indexToSpawn < TileManager.Instance.TotalTileNum - 1 && TileManager.Instance._indexToSpawn > 1)
        {
            RandomlyActivateObstacles();
        }
        else if (TileManager.Instance._indexToSpawn == TileManager.Instance.TotalTileNum - 1)
        {
            SpawnGoal();
        }
    }

    protected override void RandomlyActivateObstacles()
    {
        if (_obstacles.Length == 0 || !TileManager.Instance.bSpawnObstacles) return;
        /// disable old obstacles
        for (int i = 0; i < _numActiveObstacles; i++)
        {
            _obstacles[_activeObstacleIndex[i]].DeactivateObstacle();
            _activeObstacleIndex[i] = -1;
        }
        _numActiveObstacles = 0;

        /// randomly select new obstacles to activate
        for (int i = 0; i < _obstacles.Length; i++)
        {
            if (_numActiveObstacles == MaxNumObstacleOnCurvedBlock) break;
            int randomIndex = Random.Range(0, _obstacles.Length - 1);
            while (_obstacles[randomIndex].ActiveObsIndex != -1)
            {
                randomIndex++;
                if (randomIndex == _obstacles.Length) randomIndex = 0;
            }
            int randomVal = Random.Range(1, 100); /// for probability
            if (randomVal >= NoSpawnProbability)
            {
                _obstacles[randomIndex].ActivateARandomObstacle();
                _activeObstacleIndex[_numActiveObstacles++] = randomIndex;
            }
        }

    }

    protected override void CalculateNextTilePosRot()
    {
        Vector3 tempOffset = NextBlockOffset;
        if (_direction == Vector3.forward) { tempOffset.x *= -1f; }
        else if (_direction == Vector3.left && _curvedDir == CurvedDirection.Right) { tempOffset *= -1; }
        else if (_direction == Vector3.back) { tempOffset.z *= -1f; }
        else if (_direction == Vector3.right && _curvedDir == CurvedDirection.Left) { tempOffset *= -1; }

        NextSpawnPos = transform.position + tempOffset; /// add the offset directly
        NextSpawnRot = Quaternion.LookRotation(_direction);
    }

    public override Vector3 CalculateNextTilePos(Vector3 prevPos, Vector3 properDirection, CurvedDirection curveDir)
    {
        Vector3 tempOffset = NextBlockOffset;
        if (properDirection == Vector3.forward) { tempOffset.x *= -1f; }
        else if (properDirection == Vector3.left && curveDir == CurvedDirection.Right) { tempOffset *= -1; }
        else if (properDirection == Vector3.back) { tempOffset.z *= -1f; }
        else if (properDirection == Vector3.right && curveDir == CurvedDirection.Left) { tempOffset *= -1; }
        return prevPos + tempOffset; /// add the offset directly
    }
}

public enum CurvedDirection
{ Left, Right }

