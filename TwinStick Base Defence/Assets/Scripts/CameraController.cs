using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset;

    void Start()
    {
        offset = transform.position;
    }
    
    void Update()
    {
        transform.position = GameController.players[0].transform.position + offset;
    }
}
