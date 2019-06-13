using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform pSpawn;
    public GameObject effect;
    public Transform eSpawn;
    
    [HideInInspector] public Transform targetTransform;

    void Start()
    {
    }
    
    void Update()
    {
        data.cooldown -= Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetTransform.position,Time.deltaTime * 5);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation,Time.deltaTime * 5);
    }
}
