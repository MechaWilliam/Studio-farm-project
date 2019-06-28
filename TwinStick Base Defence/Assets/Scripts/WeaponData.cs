using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WeaponData
{
    public GameObject projectile;
    public float initialVelocity, maxCooldown, accuracy;
    public float recoil;
    public int[] burstCount; //0 = auto, 1 = single, 2+ = burst
    public int maxCapacity;
    public float mobility;
}