using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    protected GameObject[] _obstacleModels;
    protected int _activeObstacleIndex = -1;
    public int ActiveObsIndex { get { return _activeObstacleIndex; } }
    protected BoxCollider _tripCollider;

    // for Metrics
    public static int ObstaclesGenerated = 0;

    public void Initialization()
    {
        _obstacleModels = new GameObject[transform.childCount];
        for (int i=0; i<transform.childCount; i++)
        {
            _obstacleModels[i] = transform.GetChild(i).gameObject;
            _obstacleModels[i].SetActive(false);
            if (_obstacleModels[i].GetComponent<BoxCollider>()) _obstacleModels[i].GetComponent<BoxCollider>().isTrigger = true;
        }
        _activeObstacleIndex = -1;
        _tripCollider = GetComponent<BoxCollider>();
        if (_tripCollider)
        {
            _tripCollider.enabled = false;
        }
    }

    public void ActivateARandomObstacle()
    {
        _activeObstacleIndex = Random.Range(0, _obstacleModels.Length - 1);
        _obstacleModels[_activeObstacleIndex].SetActive(true);
        ObstaclesGenerated++;
        if (_tripCollider) _tripCollider.enabled = true;
    }

    public void DeactivateObstacle()
    {
        /// deactivate old object
        if (_activeObstacleIndex != -1)
        {
            _obstacleModels[_activeObstacleIndex].SetActive(false);
            _activeObstacleIndex = -1;
            if (_tripCollider) _tripCollider.enabled = false;
        }
    }

}

[System.Serializable]
public class ObjectList
{
    public ObjectList()
    {
        if (objects == null)
        {
            objects = new List<GameObject>();
            ActiveObjectIndex = -1;
        }
        else
        {
            ActiveObjectIndex = 0;
        }
    }
    public List<GameObject> objects;
    public int ActiveObjectIndex;
}