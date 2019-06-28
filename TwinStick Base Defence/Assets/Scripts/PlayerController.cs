using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Agent
{
    public Equipment equipment;
    public bool autoSwitch;

    [SerializeField] RectTransform healthBar, shieldBar;
    [SerializeField] Canvas respawnMenu;

    [SerializeField] Transform handSlot, backSlot;

    GameController gameController;

    [SerializeField] float pickupRange;

    int selectedWeapon;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < equipment.weapons.Length; i++)
        {
            var weapon = equipment.weapons[i];
            if (weapon)
            {
                weapon.equipmentIndex = i;
                weapon.equiped = true;
                weapon.agent = this;
            }
        }
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
            else if (Input.GetButtonDown("SwapWeapon"))
            {
                SwapWeapon(0, 1);
            }
        }
        else if (Input.GetButtonDown("SwapWeapon"))
        {
            SwapWeapon();
        }

        if (equipment.weapons[selectedWeapon])
        {
            if (Input.GetButton("Fire1") || (autoSwitch && Input.GetButton("Fire2")))
            {
                equipment.weapons[selectedWeapon].TriggerDown();
            }
            else if (Input.GetButtonUp("Fire1") || (autoSwitch && Input.GetButtonUp("Fire2")))
            {
                equipment.weapons[selectedWeapon].TriggerUp();
            }
            else if (Input.GetButtonDown("ToggleFireMode"))
            {
                equipment.weapons[selectedWeapon].ToggleFireMode();
            }
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (equipment.weapons[selectedWeapon] == null)
            {
                Weapon closestWeapon = null;
                float shortestdistance = Mathf.Infinity;
                foreach (Collider collider in Physics.OverlapSphere(transform.position, pickupRange))
                {
                    var weapon = collider.GetComponent<Weapon>();
                    if (weapon)
                    {
                        if ((weapon.transform.position - transform.position).magnitude < shortestdistance)
                        {
                            closestWeapon = weapon;
                        }
                    }
                }
                if (closestWeapon)
                {
                    closestWeapon.Pickup(this, selectedWeapon);
                    SelectWeapon(selectedWeapon);
                }
            }
            else
            {
                equipment.weapons[selectedWeapon].Drop(this,400);
                SelectWeapon(selectedWeapon);
            }
        }

        DisplayStats();
    }

    public void DisplayStats()
    {
        health.SetScale(healthBar);
        shield.SetScale(shieldBar);
    }

    void SwapWeapon(int weapon1, int weapon2)
    {
        if (equipment.weapons[weapon1])
        {
            if (equipment.weapons[weapon2])
            {
                var tempWeapon = equipment.weapons[weapon1];
                equipment.weapons[weapon1] = equipment.weapons[weapon2];
                equipment.weapons[weapon2] = tempWeapon;

                equipment.weapons[weapon1].equipmentIndex = weapon1;
                equipment.weapons[weapon2].equipmentIndex = weapon2;

            }
            else
            {
                equipment.weapons[weapon2] = equipment.weapons[weapon1];
                equipment.weapons[weapon2].equipmentIndex = weapon2;
                equipment.weapons[weapon1] = null;
            }
        }
        else if (equipment.weapons[weapon2])
        {
            equipment.weapons[weapon1] = equipment.weapons[weapon2];
            equipment.weapons[weapon1].equipmentIndex = weapon1;
            equipment.weapons[weapon2] = null;
        }
        SelectWeapon(selectedWeapon);
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
        selectedWeapon = index;
        foreach (Weapon weapon in equipment.weapons)
        {
            if (weapon)
            {
                if (weapon.equipmentIndex == selectedWeapon)
                {
                    weapon.targetTransform = handSlot;
                }
                else
                {
                    weapon.targetTransform = backSlot;
                    weapon.TriggerUp();
                }
            }
        }
        speed = equipment.weapons[selectedWeapon] ? maxSpeed * equipment.weapons[selectedWeapon].data.mobility : maxSpeed;
    }

    public void Heal(float health)
    {
        this.health.Add(health);
    }

    protected override void Kill()
    {
        base.Kill();
        foreach (Weapon weapon in equipment.weapons)
        {
            if (weapon)
            {
                weapon.Drop(this);
            }
        }
        gameController.EndGame();
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        DisplayStats();
        enabled = false;
    }

    public void Respawn()
    {
        enabled = true;
        transform.position = Vector3.zero;
        GetComponentInChildren<MeshRenderer>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        enabled = true;
        base.Start();
    }
}
