using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //stats
    [SerializeField] protected float speed;
    public Stat health;
    public RegenStat shield;
    protected Vector3 movement, aimDir;
    protected Weapon weapon;

    //effects
    [SerializeField] GameObject ShieldEffect;
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject killEffect;
    [SerializeField] float effectScale, effectHeight;
    [SerializeField] bool dontKill;

    //misc
    [HideInInspector] protected new Rigidbody rigidbody;
    [HideInInspector] public int id, lastHitId;

    protected virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        health.Max();
        shield.Max();
    }

    protected virtual void Update()
    {
        shield.Regen();

        transform.LookAt(transform.position + aimDir);
        if (weapon)
        {
            //weapon.transform.position = Vector3.Lerp(weapon.transform.position, transform.position + (aimDir * 1) + Vector3.up, Time.deltaTime * 5);
            //weapon.transform.LookAt(transform.position + (aimDir * 10));
        }
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(movement * speed);
    }

    public void Damage(float damage, Vector3 force, Vector3 pos)
    {
        rigidbody.AddForce(force);
        if (shield.value >= damage)
        {
            shield.value -= damage;
        }
        else
        {
            Instantiate(damageEffect, pos, Quaternion.LookRotation(-force));
            health.value -= (damage - shield.value);
            shield.value = 0f;
        }
        if (health.value <= 0f)
        {
            health.value = 0f;
            Kill();
        }
    }
    
    protected virtual void Kill()
    {
        Rigidbody effect = Instantiate(killEffect, transform.position, transform.rotation).GetComponent<Rigidbody>();
        effect.transform.localScale *= effectScale;
        effect.transform.position += transform.up * effectHeight;
        if (!dontKill)
        {
            Destroy(gameObject);
        }
    }

    protected void Shoot()
    {
        weapon.data.cooldown = weapon.data.maxCooldown;
        Vector3 dir = Quaternion.AngleAxis(Random.Range(-weapon.data.accuracy, weapon.data.accuracy), Vector3.up) * aimDir;
        Projectile projectile = Instantiate(weapon.data.projectile, weapon.pSpawn.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.transform.LookAt(weapon.pSpawn.position + dir);
        projectile.GetComponent<Rigidbody>().velocity = dir * weapon.data.initialVelocity;
        projectile.id = id;
        rigidbody.AddForce(dir * -weapon.data.recoil);
        if (weapon.effect)
        {
            Instantiate(weapon.effect, weapon.eSpawn.transform).transform.parent = null;
        }
    }
}
