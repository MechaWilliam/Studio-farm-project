using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Agent
{
    [SerializeField] Equipment equipment;
    [SerializeField] bool autoSwitch;

    [SerializeField] RectTransform healthBar, shieldBar;
    [SerializeField] Canvas respawnMenu;

    [SerializeField] Transform handSlot, backSlot;

    GameController gameController;

    int selectedWeapon;

    protected override void Start()
    {
        base.Start();
        SelectWeapon(0);
        gameController = FindObjectOfType<GameController>();
        id = 1;
    }

    protected override void Update()
    {
        base.Update();
        var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        movement = input.magnitude > 1 ? input.normalized : input;
        var screenPos = Camera.main.WorldToScreenPoint(transform.position);
        aimDir = new Vector3 (Input.mousePosition.x - screenPos.x,0f, Input.mousePosition.y - screenPos.y).normalized;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(1);
        }
        if (autoSwitch)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                SelectWeapon(0);
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                SelectWeapon(1);
            }
        }
        else if (Input.GetButtonDown("SwapWeapon"))
        {
            SwapWeapon();
        }

        if (weapon.data.cooldown <= 0 && (Input.GetButton("Fire1") || (autoSwitch && Input.GetButton("Fire2"))))
        {
            Shoot();
        }
        DisplayStats();
    }

    public void DisplayStats()
    {
        health.SetScale(healthBar);
        shield.SetScale(shieldBar);
    }

    void SwapWeapon()
    {
        if (selectedWeapon == 0)
        {
            SelectWeapon(1);
        }
        else
        {
            SelectWeapon(0);
        }
    }

    void NextWeapon()
    {

    }

    void PreviousWeapon()
    {

    }

    void SelectWeapon(int index)
    {
        switch (index)
        {
            case 0:
                weapon = equipment.primary;
                equipment.primary.targetTransform = handSlot;
                equipment.secondary.targetTransform = backSlot;
                goto default;

            case 1:
                weapon = equipment.secondary;
                equipment.primary.targetTransform = backSlot;
                equipment.secondary.targetTransform = handSlot;
                goto default;

            default:
                selectedWeapon = index;
                break;
        }
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
