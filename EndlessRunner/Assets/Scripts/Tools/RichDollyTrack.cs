using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RichDollyTrack : CinemachineSmoothPath
{
    public void AddNewPoints(WayPoint[] points)
    {
        //int maxWaypoints = 20;
        Waypoint[] newWayPoints = new Waypoint[m_Waypoints.Length + points.Length];
        //float randomOffset = Random.Range(-2f, 2f);
        for (int i = 0; i < m_Waypoints.Length; i++)
        {
            newWayPoints[i] = m_Waypoints[i];
        }
        for (int i = m_Waypoints.Length; i < m_Waypoints.Length + points.Length; i++)
        {
            newWayPoints[i].position = points[i - m_Waypoints.Length].transform.position;
        }
        m_Waypoints = newWayPoints;
    }

    public void CreatePath(List<Vector3> points)
    {
        Waypoint[] newWayPoints = new Waypoint[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            newWayPoints[i].position = points[i];
        }
        m_Waypoints = newWayPoints;
    }
}
