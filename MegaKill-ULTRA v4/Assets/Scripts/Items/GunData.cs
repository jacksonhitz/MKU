using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Items/Gun")]
public class GunData : ItemData
{
    public float dmg;
    public float fireRate;
    public float maxBullets;
    public float recoilMag;
    public float recoilSpd;

    public ParticleSystem muzzleFlash;

    //mg-sg
    public float spreadAngle;

    //sg
    public float pellets;
}
