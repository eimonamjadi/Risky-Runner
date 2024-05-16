using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSpawnNewTiles : MonoBehaviour
{
    [HideInInspector] public bool bNextTileSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !bNextTileSpawned)
        {
            bNextTileSpawned = true;
            TileManager.Current.SpawnTile();
        }
    }
}
