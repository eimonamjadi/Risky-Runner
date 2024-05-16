using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileType { Forward, LeftCurve, RightCurve };
public struct TileStruct
{
    public TileStruct(TileType _t, Vector3 _p, Quaternion _r) { _type = _t; _pos = _p; _rot = _r; }
    public TileType _type;
    public Vector3 _pos;
    public Quaternion _rot;
}

public class TileManager : MMSingleton<TileManager>
{
    [Header("TilePrefabs")]
    public TileForward forwardTilePrefab;
    protected WayPoint[] forwardWayPoints;
    protected int _chanceSpawnForward = 70;
    public TileCurved leftCurvedTilePrefab;
    protected WayPoint[] leftWayPoints;
    protected int _chanceSpawnLeft = 15;
    public TileCurved rightCurvedTilePrefab;
    protected WayPoint[] rightWayPoints;
    protected List<Vector3> wayPoints;
    protected int _chanceSpawnRight = 15;
    protected TileStruct[] _precalculatedTiles;
    protected const int _totalTileNum = 28;
    public int TotalTileNum { get { return _totalTileNum; } }
    public List<TileMod> _activeTiles;
    public List<TileForward> _inactiveForwardTiles;
    public List<TileCurved> _inactiveLeftCurveTiles;
    public List<TileCurved> _inactiveRightCurveTiles;
	public int _numTiles = 8;
    protected TileType _lastTileType;
    protected TileMod _lastTile;
    public int _indexToSpawn = 0;
    public bool bSpawnObstacles = true;
    public int MaxObstaclesPerLevel { get; set; }
    public Vector3 _worldDir;

    public RichDollyTrack _track;
    [HideInInspector] public bool bInitialized = false;

    [Header("for METRICS purposes")]
    public static int StraightTilesNum = 0;
    public static int CurvedTilesNum = 0;

    public void Initialization(int newTargetSpeed)
    {
        if (bInitialized) return;
        if (FindObjectOfType<PlayerController>())
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            controller.TargetMoveSpeed = newTargetSpeed;
        }
        else if (FindObjectOfType<NewPlayerController>())
        {
            NewPlayerController controller = FindObjectOfType<NewPlayerController>();
            controller.TargetMoveSpeed = newTargetSpeed;

            if (SpeedSlider.HasInstance) SpeedSlider.Current.SetTargetValue(newTargetSpeed);
            controller.RotationRate = 10f;

            // for metrics
            if (controller.Speeds == null) controller.Speeds = new List<int>();
            controller.Speeds.Add(newTargetSpeed);

            // for metrics
            if (controller.RandomHeadingChances == null) controller.RandomHeadingChances = new List<float>();
            controller.RandomHeadingChances.Add(controller.RandomHeadingChance);
        }

        _activeTiles = new List<TileMod>();
        _inactiveForwardTiles = new List<TileForward>();
        _inactiveLeftCurveTiles = new List<TileCurved>();
        _inactiveRightCurveTiles = new List<TileCurved>();
        wayPoints = new List<Vector3>();
        forwardWayPoints = forwardTilePrefab.GetComponentsInChildren<WayPoint>();
        leftWayPoints = leftCurvedTilePrefab.GetComponentsInChildren<WayPoint>();
        rightWayPoints = rightCurvedTilePrefab.GetComponentsInChildren<WayPoint>();

        StraightTilesNum = 0;
        CurvedTilesNum = 0;

        PreCalculateTiles();

        /// Create a Curved Dolly Track
        foreach (TileStruct tile in _precalculatedTiles)
        {
            Transform tileTransform = forwardTilePrefab.transform;
            switch (tile._type)
            {
                case TileType.Forward:
                    forwardTilePrefab.transform.rotation = tile._rot;
                    forwardTilePrefab.transform.position = tile._pos;
                    foreach (WayPoint pt in forwardWayPoints)
                    {
                        Vector3 pos = pt.transform.position;
                        wayPoints.Add(pos);
                    }
                    break;
                case TileType.LeftCurve:
                    leftCurvedTilePrefab.transform.rotation = tile._rot;
                    leftCurvedTilePrefab.transform.position = tile._pos;
                    foreach (WayPoint pt in leftWayPoints)
                    {
                        Vector3 pos = pt.transform.position;
                        wayPoints.Add(pos);
                    }
                    break;
                case TileType.RightCurve:
                    rightCurvedTilePrefab.transform.rotation = tile._rot;
                    rightCurvedTilePrefab.transform.position = tile._pos;
                    foreach (WayPoint pt in rightWayPoints)
                    {
                        Vector3 pos = pt.transform.position;
                        wayPoints.Add(pos);
                    }
                    break;
                default:
                    break;
            }
        }
        _track.CreatePath(wayPoints);

        /// Calculate max number of obstacles on each block
        if (Loader.Instance.LoadingFixedSpeedLevel)
        {
            TileMod.MaxNumObstacleOnOneBlock = Mathf.Max(0, Mathf.FloorToInt(MaxObstaclesPerLevel / (StraightTilesNum + 1.5f * CurvedTilesNum)));
            TileCurved.MaxNumObstacleOnCurvedBlock = Mathf.Max(0, Mathf.RoundToInt(1.5f * MaxObstaclesPerLevel / (StraightTilesNum + 1.5f * CurvedTilesNum)));
        }
        else
        {
            TileMod.MaxNumObstacleOnOneBlock = 2;
            TileCurved.MaxNumObstacleOnCurvedBlock = 3;
            
        }

        /// instantiate the blocks
        for (int i = 0; i < _numTiles; i++)
        {
            SpawnTile();
        }

        SpeedDisplay speed = FindObjectOfType<SpeedDisplay>();
        if (speed != null)
        {
            if (FindObjectOfType<PlayerController>())
            {
                PlayerController controller = FindObjectOfType<PlayerController>();
                speed.GetComponent<Text>().text = controller.TargetMoveSpeed.ToString("00 mi/h");
            }
            else if (FindObjectOfType<NewPlayerController>())
            {
                NewPlayerController controller = FindObjectOfType<NewPlayerController>();
                speed.GetComponent<Text>().text = controller.TargetMoveSpeed.ToString("00 mi/h");
            }
        }

        bInitialized = true;
    }

    private void Start()
    {
        if (Loader.Instance == null  || (!bInitialized && !Loader.Instance.LevelInitialized)) Initialization(Loader.DefaultSpeed);
    }

    public virtual IEnumerator SpawnTileCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpawnTile();
    }

    [ContextMenu("SpawnTile")]
    /// <summary>
    /// Spawn a random tile
    /// </summary>
    public virtual void SpawnTile()
    {
        if (_indexToSpawn == _totalTileNum) return;
        /// randomly spawn environments
        PickAPrefabToSpawn();
        _indexToSpawn++;

        /// activate the collider on the last tile to prevent the player from going backward
        if (_activeTiles.Count == Mathf.Min(_totalTileNum, _numTiles))
        {
            _activeTiles[0].PreventGoingBackCollider.SetActive(true);
        }
    }

    /// <summary>
    /// Spawn a tile of your choice
    /// </summary>
    /// <param name="tileToSpawn"></param>
    public virtual void SpawnTile(TileMod tileToSpawn)
    {
        /// select from the pool
        TileMod tileMod;
        tileMod = SelectFromPool(tileToSpawn);

        /// if no appropriate tile exists in the pool, instantiate a new one
        if (tileMod == null)
        {
            tileMod = Instantiate(tileToSpawn, _precalculatedTiles[_indexToSpawn]._pos, _precalculatedTiles[_indexToSpawn]._rot);
            _activeTiles.Add(tileMod);
            _lastTile = tileMod;
        }

        /// deactivate tiles that are unnecessary
        if (_activeTiles.Count > _numTiles)
        {
            DeactivateTile();
        }
    }

    /// <summary>
    /// Deactivate the last tile
    /// </summary>
    protected virtual void DeactivateTile()
    {
        if (_activeTiles.Count == 0) return;
        TileMod tileToRemove = _activeTiles[0];
        _activeTiles.RemoveAt(0);
        /// determine what type of tile it is and put it to the correct pool
        if (tileToRemove is TileForward) { _inactiveForwardTiles.Add((TileForward)tileToRemove); }
        else if (tileToRemove is TileCurved)
        {
            if (((TileCurved)tileToRemove)._curvedDir == CurvedDirection.Left) { _inactiveLeftCurveTiles.Add((TileCurved)tileToRemove); }
            if (((TileCurved)tileToRemove)._curvedDir == CurvedDirection.Right) { _inactiveRightCurveTiles.Add((TileCurved)tileToRemove); }
        }
        tileToRemove.gameObject.SetActive(false);
    }

    protected virtual void PickAPrefabToSpawn()
    {
        if (_precalculatedTiles[_indexToSpawn]._type == TileType.Forward) SpawnTile(forwardTilePrefab);
        else if (_precalculatedTiles[_indexToSpawn]._type == TileType.LeftCurve) SpawnTile(leftCurvedTilePrefab);
        else SpawnTile(rightCurvedTilePrefab);
    }

    /// <summary>
    /// Pick a random curve or forward tile, make sure the path doesn't go backward in the world space
    /// </summary>
    /// <returns></returns>
    protected TileType PickRandomTileType(int i)
    {
        int num = Random.Range(1, 100);
        TileType retType = TileType.Forward;
        if (num < _chanceSpawnForward)
        {
            retType = TileType.Forward;
        }
        else if (num < _chanceSpawnForward + _chanceSpawnLeft)
        {
            if (_worldDir == Vector3.left) retType = TileType.RightCurve;
            else retType = TileType.LeftCurve;
        }
        else if (num < _chanceSpawnForward + _chanceSpawnLeft + _chanceSpawnRight)
        {
            if (_worldDir == Vector3.right) retType = TileType.LeftCurve;
            else retType = TileType.RightCurve;
        }
        if (retType == TileType.LeftCurve && _lastTileType == TileType.LeftCurve) retType = TileType.RightCurve;
        else if (retType == TileType.RightCurve && _lastTileType == TileType.RightCurve) retType = TileType.LeftCurve;

        return retType;
    }

    protected TileMod SelectFromPool(TileMod tileType)
    {
        if (tileType is TileForward && _inactiveForwardTiles.Count > 0)
        {
            _inactiveForwardTiles[0].ResetPosition(_precalculatedTiles[_indexToSpawn]._pos, _precalculatedTiles[_indexToSpawn]._rot);
            _inactiveForwardTiles[0].gameObject.SetActive(true);
            _lastTile = _inactiveForwardTiles[0];
            _activeTiles.Add(_lastTile);
            _inactiveForwardTiles.RemoveAt(0);
            return _lastTile;
        }
        else if (tileType is TileCurved && ((TileCurved)tileType)._curvedDir == CurvedDirection.Left && _inactiveLeftCurveTiles.Count > 0)
        {
            _inactiveLeftCurveTiles[0].ResetPosition(_precalculatedTiles[_indexToSpawn]._pos, _precalculatedTiles[_indexToSpawn]._rot);
            _inactiveLeftCurveTiles[0].gameObject.SetActive(true);
            _lastTile = _inactiveLeftCurveTiles[0];
            _activeTiles.Add(_lastTile);
            _inactiveLeftCurveTiles.RemoveAt(0);
            return _lastTile;
        }
        else if (tileType is TileCurved && ((TileCurved)tileType)._curvedDir == CurvedDirection.Right && _inactiveRightCurveTiles.Count > 0)
        {
            _inactiveRightCurveTiles[0].ResetPosition(_precalculatedTiles[_indexToSpawn]._pos, _precalculatedTiles[_indexToSpawn]._rot);
            _inactiveRightCurveTiles[0].gameObject.SetActive(true);
            _lastTile = _inactiveRightCurveTiles[0];
            _activeTiles.Add(_lastTile);
            _inactiveRightCurveTiles.RemoveAt(0);
            return _lastTile;
        }
        return null;
    }

    protected void PreCalculateTiles()
    {
        _precalculatedTiles = new TileStruct[_totalTileNum];
        /// precalculate all the tiles
        Vector3 nextTilePos = Vector3.back * 20f;
        Quaternion nextTileRot = Quaternion.identity;
        _worldDir = Vector3.forward;
        _lastTileType = TileType.Forward;

        _precalculatedTiles[0] = new TileStruct(TileType.Forward, nextTilePos, nextTileRot);
        nextTilePos = forwardTilePrefab.CalculateNextTilePos(nextTilePos, _worldDir, CurvedDirection.Left);
        nextTileRot = Quaternion.LookRotation(_worldDir);

        for (int i = 1; i < _totalTileNum; i++)
        {
            TileType nextType = PickRandomTileType(i - 1); // this function also yields a new '_worldDir'
            _precalculatedTiles[i] = new TileStruct(nextType, nextTilePos, nextTileRot);

            if (_worldDir == Vector3.forward)
            {
                if (nextType == TileType.LeftCurve) _worldDir = Vector3.left;
                else if (nextType == TileType.RightCurve) _worldDir = Vector3.right;
            }
            else if (_worldDir == Vector3.left)
            {
                if (nextType == TileType.RightCurve) _worldDir = Vector3.forward;
            }
            else if (_worldDir == Vector3.right)
            {
                if (nextType == TileType.LeftCurve) _worldDir = Vector3.forward;
            }

            // calculate next block positions
            switch (_precalculatedTiles[i]._type)
            {
                case TileType.Forward:
                    nextTilePos = forwardTilePrefab.CalculateNextTilePos(nextTilePos, _worldDir, CurvedDirection.Left); // CurvedDirection.Left is useless in this case
                    break;
                case TileType.LeftCurve:
                    nextTilePos = leftCurvedTilePrefab.CalculateNextTilePos(nextTilePos, _worldDir, CurvedDirection.Left);
                    break;
                case TileType.RightCurve:
                    nextTilePos = rightCurvedTilePrefab.CalculateNextTilePos(nextTilePos, _worldDir, CurvedDirection.Right);
                    break;
                default:
                    break;
            }
            nextTileRot = Quaternion.LookRotation(_worldDir);

            _lastTileType = nextType;

            // For Metrics
            if (_lastTileType == TileType.Forward)
            {
                StraightTilesNum++;
            }
            else
            {
                CurvedTilesNum++;
            }
        }
    }
}
