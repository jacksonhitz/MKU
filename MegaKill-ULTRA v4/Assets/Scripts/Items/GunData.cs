using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Items/Gun")]
public class GunData : WeaponData
{
    //MAKE A WEAPONDATA THAT GUNDATA INHIERTS FROM FOR MELEE FUCKER

    [Header("Universal")]
    public float dmg;
    public float maxBullets;
    public float recoilMag;
    public float recoilSpd;
    public float vel;

    [Header("Enemy")]
    public GameObject bulletPrefab;
    public float range;

    [Header("MG/SG")]
    public float spreadAngle;

    [Header("SG")]
    public float pellets;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public TrailRenderer tracerPrefab;
    public float tracerDuration;
}
