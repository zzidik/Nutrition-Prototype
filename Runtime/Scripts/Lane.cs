using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [Serializable]
    public class WayPoint
    {
        public GameObject go;
        public bool open = false;

    }
    public Runner runner;
    public List<WayPoint> waypoints;
    public int openWayPoint = 0;
    

    /// <summary>
    /// Gathers the wayPoints of the Lane into a list.
    /// Opens the first one.
    /// </summary>
    private void OnEnable()
    {
        foreach (Transform child in transform)
        {
            WayPoint wp = new WayPoint();
            wp.go = child.gameObject;
            waypoints.Add(wp);
        }

        waypoints[0].open = true;
        openWayPoint = 0;
    }

    /// <summary>
    /// Opens the next waypoint in the Lane.
    /// </summary>
    public void OpenNextWayPoint()
    {
        int numWayPoints = waypoints.Count;

        // at end of track
        if (openWayPoint == numWayPoints - 1)
        {
            return;            
        }
        else
        {
            openWayPoint++;
            waypoints[openWayPoint].open = true;
        }
    }

    /// <summary>
    /// Closes all the waypoints, opens the first one.
    /// Tells the runner to go back to the first waypoint.
    /// </summary>
    public void Reset()
    {
        int numWayPoints = waypoints.Count;
        for (int wp = 0; wp < numWayPoints; wp++)
        {
            waypoints[wp].open = false;
        }

        waypoints[0].open = true;
        openWayPoint = 0;

        runner.Reset();
    }
}
