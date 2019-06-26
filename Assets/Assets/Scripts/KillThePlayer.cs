using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillThePlayer : MonoBehaviour
{
    private Transform target;

    [Header("Attributes")]

    public float firerate = 1f;
    private float firecountdown = 0f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (firecountdown <= 0f)
        {
            Shoot();
            firecountdown = 1f / firerate;
        }

        firecountdown -= Time.deltaTime;
    }

    void Shoot()
    {
     GameObject bulletGO =  (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if(bullet != null)
        {
            bullet.Seek(target);
        }
     }
}
