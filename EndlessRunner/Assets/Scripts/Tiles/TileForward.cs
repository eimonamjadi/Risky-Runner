using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileForward : TileMod
{
    public float _tileLength = 20f;

    protected override void CalculateNextTilePosRot()
    {
        NextSpawnPos = transform.position + _direction * _tileLength;
        NextSpawnRot = transform.rotation;
    }

    public override Vector3 CalculateNextTilePos(Vector3 prevPos, Vector3 prevWorldDir, CurvedDirection curveDir)
    {
        return prevPos + prevWorldDir * _tileLength;
    }
}
