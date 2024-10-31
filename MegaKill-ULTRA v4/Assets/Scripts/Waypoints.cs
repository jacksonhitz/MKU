using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;
    // Start is called before the first frame update
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
    // Check if there are any waypoints
    if (transform.childCount == 0) 
    {
        Debug.LogWarning("No waypoints available.");
        return null;
    }

    // Find the index of the current waypoint
    int currentIndex = -1;
    for (int i = 0; i < transform.childCount; i++)
    {
        if (transform.GetChild(i) == currentWaypoint)
        {
            currentIndex = i;
            break;
        }
    }

    // If the current waypoint is not found, return the first waypoint as the default
    if (currentIndex == -1)
    {
        Debug.LogWarning("Current waypoint not found in list. Returning first waypoint.");
        return transform.GetChild(0);
    }

    // Get the next waypoint index (wrap around if needed)
    int nextIndex = (currentIndex + 1) % transform.childCount;
    return transform.GetChild(nextIndex);
    }
}
