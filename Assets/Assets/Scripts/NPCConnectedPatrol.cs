using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Text;



public class NPCConnectedPatrol : MonoBehaviour
{
    //dictates whether the agent waits on each node.
    [SerializeField]
    bool _patrolWaiting;

    //the total time we wait at each node
    [SerializeField]
    float _totalWaitTime = 3f;

    //the probability of switching direction
    [SerializeField]
    float _switchProbability = 0.2f;

    //private varibles fot base behaviour
    NavMeshAgent _navMeshAgent;
    ConnectedWayPoints _currentWaypoint;
    ConnectedWayPoints _previousWaypoint;

    bool _travelling;
    bool _waiting;
    float _waitTimer;
    int _waypointsVisited;

    public void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            if (_currentWaypoint == null)
            {
                //set it at random
                //grab all waypoint objects in scene.
                GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

                if (allWaypoints.Length == null)
                {
                    while (_currentWaypoint == null)
                    {
                        int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                        ConnectedWayPoints startingWaypoint = allWaypoints[random].GetComponent<ConnectedWayPoints>();

                        //ie we found a waypoint
                        if (startingWaypoint != null)
                        {
                            _currentWaypoint = startingWaypoint;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to find any waypoints for use in the scene.");
                }
            }

            SetDestination();
        }
    }

    public void Update()
    {
        //check if we're close to the destination
        if (_travelling && _navMeshAgent.remainingDistance <= 1.0f)
        {
            _travelling = false;
            _waypointsVisited++;

            //if we're going to wait,then wait
            if (_patrolWaiting)
            {
                _waiting = true;
                _waitTimer = 0f;
            }
            else
            {
                SetDestination();
            }
        }
        //instead if we're waiting
        if (_waiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _totalWaitTime)
            {
                _waiting = false;

                SetDestination();
            }
        }
    }

    private void SetDestination()
    {
        if (_waypointsVisited > 0)
        {
            ConnectedWayPoints nextWaypoint = _currentWaypoint.NextWaypoint(_previousWaypoint);
            _previousWaypoint = _currentWaypoint;
            _currentWaypoint = nextWaypoint;
        }

        Vector3 targetVector = _currentWaypoint.transform.position;
        _navMeshAgent.SetDestination(targetVector);
        _travelling = true;
    }
}


