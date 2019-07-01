using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WeaponData
{
    public GameObject projectile;
    public float initialVelocity,  accuracy;
    public float recoil;
    public int[] burstCount; //0 = auto, 1 = single, 2+ burst
    public float[] maxCooldown; //for each firemode
    public int maxCapacity;
    public float mobility;
}