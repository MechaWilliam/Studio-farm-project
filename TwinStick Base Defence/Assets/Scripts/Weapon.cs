using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform pSpawn;
    public GameObject effect;
    public float effectDelay;
    public Transform eSpawn;
    [HideInInspector] public bool equiped = false;
    [HideInInspector] public int equipmentIndex;
    [HideInInspector] public Transform targetTransform;
    [HideInInspector] public Agent agent;

    [SerializeField] bool ignorObstruction;

    Animator animator;

    float cooldown;
    int burstIndex;
    int fireMode;
    bool triggerDown;
    int capacity;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (equiped && targetTransform)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * 5);
        }
        if (cooldown <= 0)
        {
            if (burstIndex > 0)
            {
                burstIndex--;
                Shoot();
            }
            else if (data.burstCount[fireMode] == 0 && triggerDown)
            {
                Shoot();
            }
        }
    }

    public void TriggerDown()
    {
        if (!triggerDown)
        {
            if (data.burstCount[fireMode] > 0)
            {
                burstIndex = data.burstCount[fireMode];
            }
            triggerDown = true;
        }
    }

    public void TriggerUp()
    {
        if (triggerDown)
        {
            if (burstIndex <= 1)
            {
                burstIndex = 0;
            }
            triggerDown = false;
        }
    }

    public void ToggleFireMode()
    {
        if (fireMode < data.burstCount.Length - 1)
        {
            fireMode++;
        }
        else
        {
            fireMode = 0;
        }
    }

    void Shoot()
    {
        if (ignorObstruction || !Physics.CheckSphere(pSpawn.position, 0.25f, ~(1<<10)))
        {
            cooldown = data.maxCooldown[fireMode];
            Vector3 dir = Quaternion.AngleAxis(Random.Range(-data.accuracy, data.accuracy), Vector3.up) * (agent ? agent.aimDir : transform.rotation * Vector3.forward);
            Vector3 aimPos = agent ? agent.transform.position + (dir * 10f) : pSpawn.position + dir;
            aimPos.y = transform.position.y;
            Projectile projectile = Instantiate(data.projectile, pSpawn.position, Quaternion.identity).GetComponent<Projectile>();
            projectile.transform.LookAt(aimPos);
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * data.initialVelocity;
            projectile.id = (agent && !ignorObstruction) ? agent.id : 100;
            if (agent)
            {
                agent.rigidbody.AddForce(dir * -data.recoil);
            }
            if (effect)
            {
                Invoke("DoEffect", effectDelay);
            }
            if (animator)
            {
                animator.SetBool("Trigger", true);
            }
        }
    }


    public void Pickup(PlayerController player, int slot, bool switchTo = false)
    {
        equiped = true;
        player.equipment.weapons[slot] = this;
        equipmentIndex = slot;
        transform.parent = player.transform;
        agent = player;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
        if(switchTo || player.selectedWeapon == equipmentIndex)
        {
            player.SelectWeapon(equipmentIndex);
        }
    }

    public void Drop(PlayerController player, float force = 0)
    {
        player.equipment.weapons[equipmentIndex] = null;
        Drop(force);
    }

    void Drop(float force)
    {
        equiped = false;
        transform.parent = null;
        GetComponent<BoxCollider>().enabled = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(agent.aimDir * force);
        rigidbody.AddTorque(new Vector3(200, 500, 200));
        TriggerUp();
    }

    public void DoEffect()
    {
        Instantiate(effect, eSpawn.transform).transform.parent = null;
    }
}