using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ConnectedWayPoints : MonoBehaviour
{
    [SerializeField]
    protected float _connectivityRadius = 50f;

    List<ConnectedWayPoints> _connections;

    // Start is called before the first frame update
    public void Start()
    {
        //grab all waypoint objects in scene
        GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        //create a list of waypouints
        _connections = new List<ConnectedWayPoints>();

        //check if they are connected waypoints
        for (int i = 0; i < allWaypoints.Length; i++)
        {
            ConnectedWayPoints nextWaypoint = allWaypoints[i].GetComponent<ConnectedWayPoints>();

            //ie we found a waypoint
            if (nextWaypoint != null)
            {
                if (Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= _connectivityRadius && nextWaypoint != this)
                {
                    _connections.Add(nextWaypoint);
                }
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _connectivityRadius);
    }

    public ConnectedWayPoints NextWaypoint(ConnectedWayPoints previousWaypoint)
    {
        if (_connections.Count == 0)
        {
            //no waypoints? return null and complain
            Debug.LogError("Insufficient waypoint count.");
            return null;
        }
        else if (_connections.Count == 1 && _connections.Contains(previousWaypoint))
        {
            //only one waypoint and its the previous one, use that
            return previousWaypoint;
        }
        else //otherwise, find a random one that isnt the previous one.
        {
            ConnectedWayPoints nextWaypoint;
            int nextIndex = 0;

            do
            {
                nextIndex = UnityEngine.Random.Range(0, _connections.Count);
                nextWaypoint = _connections[nextIndex];

            } while (nextWaypoint == previousWaypoint);

            return nextWaypoint;
        }
    }

}


