using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Agent
{
    [SerializeField] WeaponData[] equipment;
    [SerializeField] Transform pSpawn;

    [SerializeField] RectTransform healthBar, shieldBar;
    [SerializeField] Canvas respawnMenu;

    GameController gameController;

    protected override void Start()
    {
        base.Start();
        gameController = FindObjectOfType<GameController>();
        id = 1;
    }

    protected override void Update()
    {
        base.Update();
        movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);
        aimDir = new Vector3 (Input.mousePosition.x - screenPos.x,0f, Input.mousePosition.y - screenPos.y).normalized;

        foreach (WeaponData weapon in equipment)
        {
            weapon.cooldown -= Time.deltaTime;
        }
        if (equipment[0].cooldown <= 0 && Input.GetButton("Fire1"))
        {
            Shoot(0);
        }
        else if (equipment[1].cooldown <= 0 && Input.GetButton("Fire2"))
        {
            Shoot(1);
        }
        DisplayStats();
    }

    public void DisplayStats()
    {
        health.SetScale(healthBar);
        shield.SetScale(shieldBar);
    }

    void Shoot(int weaponIndex)
    {
        WeaponData weaponData = equipment[weaponIndex];
        Vector3 dir = Quaternion.AngleAxis(Random.Range(-weaponData.accuracy, weaponData.accuracy), Vector3.up) * aimDir;
        weaponData.cooldown = weaponData.maxCooldown;
        Projectile projectile = Instantiate(weaponData.projectile, pSpawn.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.transform.LookAt(pSpawn.position + dir);
        projectile.GetComponent<Rigidbody>().velocity = dir * weaponData.initialVelocity;
        projectile.id = id;
        rigidbody.AddForce(dir * -weaponData.recoil);
    }

    public void Heal(float health)
    {
        this.health.Add(health);
    }

    protected override void Kill()
    {
        base.Kill();
        respawnMenu.enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        DisplayStats();
        enabled = false;
    }

    public void Respawn()
    {
        enabled = true;
        respawnMenu.enabled = false;
        transform.position = Vector3.zero;
        GetComponentInChildren<MeshRenderer>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        enabled = true;
        gameController.ResetScore();
        gameController.StartWave(1);
        base.Start();
    }
}
