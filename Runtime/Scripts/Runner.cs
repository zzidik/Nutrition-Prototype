using System;
using UnityEngine;
using DG.Tweening;

public class Runner : MonoBehaviour
{
    public Lane lane;
    public float speed = 1f;
    public int currentWayPoint = 0;

    /// <summary>
    /// The Tween of the position.
    /// </summary>
    public Tween positionTween;

    public Tween rotationTween;


    private void Update()
    {
        if (!positionTween.IsActive())
        {
            if(lane.openWayPoint != currentWayPoint)
            {
                CheckForOpenWayPoint();
            }
        }
    }

    public void CheckForOpenWayPoint()
    {
        int numWayPoints = lane.waypoints.Count;

        // at end of track
        if (currentWayPoint == lane.waypoints.Count - 1)
        {
            return;
        }
        else
        {
            if (lane.waypoints[currentWayPoint + 1].open)
            {
                currentWayPoint++;
                MoveToOpenWayPoint();
            }
        }
    }

    public void MoveToOpenWayPoint()
    {
        positionTween?.Kill();
        positionTween = transform.DOMove(lane.waypoints[currentWayPoint].go.transform.position, speed).SetEase(Ease.Linear);

        rotationTween?.Kill();
        rotationTween = transform.DORotate(lane.waypoints[currentWayPoint].go.transform.eulerAngles, speed).SetEase(Ease.Linear);

    }

    public void Reset()
    {
        positionTween?.Kill();
        rotationTween?.Kill();

        currentWayPoint = 0;
        transform.position = lane.waypoints[0].go.transform.position;
        transform.eulerAngles = lane.waypoints[0].go.transform.eulerAngles;
    }
}
