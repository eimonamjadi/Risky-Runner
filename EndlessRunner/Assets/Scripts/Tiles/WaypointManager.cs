using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WaypointManager : MMSingleton<WaypointManager>
{
    protected LinkedListNode<Transform> _activeTarget;
    protected LinkedList<Transform> _waypoints;
    protected PlayerController _player;
    public CinemachineSmoothPath _track;
    
    public float waypointThreshold = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        _waypoints = new LinkedList<Transform>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    protected virtual void Update()
    {
        if (_activeTarget == null) return;
        Debug.Log(Vector3.Distance(_activeTarget.Value.position, _player.transform.position));
        if (Vector3.Distance(_activeTarget.Value.position, _player.transform.position) <= waypointThreshold)
        {
            _activeTarget = _activeTarget.Next;
            //_player.SetNewTarget(_activeTarget.Value);
        }
    }

    public virtual void AddNode(WayPoint pt)
    {
        LinkedListNode<Transform> newNode = new LinkedListNode<Transform>(pt.transform);
        _waypoints.AddLast(newNode);
        
        if (_activeTarget == null)
        {
            _activeTarget = newNode;
            _player.transform.position = _activeTarget.Value.position;
        }
    }
}
