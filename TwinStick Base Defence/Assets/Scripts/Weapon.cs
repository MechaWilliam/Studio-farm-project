using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform pSpawn;
    public GameObject effect;
    public Transform eSpawn;
    [HideInInspector] public bool equiped = true;
    [HideInInspector] public int equipmentIndex;
    
    [HideInInspector] public Transform targetTransform;

    void Start()
    {

    }
    
    void Update()
    {
        data.cooldown -= Time.deltaTime;
        if (equiped)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * 5);
        }
    }
    
    public void Shoot(Agent agent, Vector3 aimDir, bool AddRecoil = true)
    {
        data.cooldown = data.maxCooldown;
        Vector3 dir = Quaternion.AngleAxis(Random.Range(-data.accuracy, data.accuracy), Vector3.up) * aimDir;
        Projectile projectile = Instantiate(data.projectile, pSpawn.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.transform.LookAt(pSpawn.position + dir);
        projectile.GetComponent<Rigidbody>().velocity = dir * data.initialVelocity;
        projectile.id = agent.id;
        if (AddRecoil)
        {
            agent.rigidbody.AddForce(dir * -data.recoil);
        }
        if (effect)
        {
            Instantiate(effect, eSpawn.transform).transform.parent = null;
        }
    }

    public void Pickup(PlayerController player, int slot)
    {
        equiped = true;
        player.equipment.weapons[slot] = this;
        equipmentIndex = slot;
        transform.parent = player.transform;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
    }

    public void Drop(PlayerController player, Vector3 aimDir)
    {
        player.equipment.weapons[equipmentIndex] = null;
        Drop(aimDir);
    }

    void Drop(Vector3 aimDir)
    {
        equiped = false;
        transform.parent = null;
        GetComponent<BoxCollider>().enabled = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(aimDir * 500);
        rigidbody.AddTorque(new Vector3(0, 500, 0));
    }
}