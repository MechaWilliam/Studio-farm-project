using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Agent
{
    [SerializeField] float damage, force, maxAttackCooldown;
    float attackCooldown;
    PlayerController player;
    
    [SerializeField] int points;
    [SerializeField] GameObject drop;

    protected override void Start()
    {
        base.Start();
        id = Random.Range(1000, 10000);
        player = GameController.players[0];
        speed = maxSpeed;
    }

    protected override void Update()
    {
        base.Update();
        attackCooldown -= Time.deltaTime;
        if (player.isActiveAndEnabled)
        {
            movement = (player.transform.position - transform.position).normalized;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ( collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<Agent>().Damage(damage, (player.transform.position - transform.position).normalized * force,collision.GetContact(0).point);
            attackCooldown = maxAttackCooldown;
        }
    }

    protected override void Kill()
    {
        base.Kill();
        if (!GameController.dontScore)
        {
            FindObjectOfType<GameController>().AddScore(points);
            if (drop)
            {
                Instantiate(drop, transform.position, Quaternion.identity);
            }
        }
    }
}
