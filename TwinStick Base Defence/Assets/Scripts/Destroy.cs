using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float killTimer;
    void Start()
    {
        Destroy(gameObject, killTimer);
    }
}