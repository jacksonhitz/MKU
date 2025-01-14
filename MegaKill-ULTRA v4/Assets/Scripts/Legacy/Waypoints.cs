using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;
    private void OnDrawGizmos()
    {
        foreach(Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, waypointSize);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1; i ++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }
    public Transform GetNextWaypoints(Transform currentWaypoint)
    {
        if (transform.childCount == 0) 
        {
            Debug.LogWarning("No waypoints available.");
            return null;
        }

        int currentIndex = -1;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i) == currentWaypoint)
            {
                currentIndex = i;
                break;
            }
        }
        if (currentIndex == -1)
        {
            return transform.GetChild(0);
        }

        int nextIndex = (currentIndex + 1) % transform.childCount;
        return transform.GetChild(nextIndex);
    }
}
