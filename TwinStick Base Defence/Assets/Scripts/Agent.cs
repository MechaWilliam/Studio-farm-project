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
    
    //effects
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject killEffect;
    [SerializeField] float effectScale, effectHeight;
    [SerializeField] bool dontKill;

    //misc
    [HideInInspector] protected Rigidbody rigidbody;
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
}
