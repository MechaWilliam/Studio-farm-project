using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WeaponData
{
    public GameObject projectile;
    public float initialVelocity, maxCooldown, accuracy;
    public float recoil;
    [HideInInspector] public float cooldown;
}