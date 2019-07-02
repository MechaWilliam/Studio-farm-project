using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //stats
    [SerializeField] protected float maxSpeed;
    protected float speed;
    public Stat health;
    public RegenStat shield;
    [HideInInspector] public Vector3 movement, aimDir;

    //effects
    [SerializeField] GameObject ShieldEffect;
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject killEffect;
    [SerializeField] float effectScale, effectHeight;
    [SerializeField] bool dontKill;

    //misc
    [HideInInspector] public new Rigidbody rigidbody;
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
        if (Time.timeScale == 1)
        {
            transform.LookAt(transform.position + aimDir);
        }
        //weapon.transform.position = Vector3.Lerp(weapon.transform.position, transform.position + (aimDir * 1) + Vector3.up, Time.deltaTime * 5);
        //weapon.transform.LookAt(transform.position + (aimDir * 10));
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(movement * speed);
    }

    public void Damage(float damage, Vector3 force, Vector3 pos)
    {
        if (shield.value >= damage)
        {
            shield.value -= damage;
        }
        else
        {
            Instantiate(damageEffect, pos, force == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(-force));
            health.value -= (damage - shield.value);
            shield.value = 0f;
        }
        if (health.value <= 0f)
        {
            health.value = 0f;
            Kill();
        }
        else
        {
            rigidbody.AddForce(force);
        }
    }

    protected virtual void Kill()
    {
        Rigidbody effect = Instantiate(killEffect, transform.position, transform.rotation).GetComponent<Rigidbody>();
        effect.transform.localScale *= effectScale;
        effect.transform.position += transform.up * effectHeight;
        if (!dontKill)
        {
            Destroy(gameObject, 0.05f);
        }
    }
}
