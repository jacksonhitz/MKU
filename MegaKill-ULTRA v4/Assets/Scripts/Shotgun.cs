using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public float shells = 2f;
    public int pellets = 12;
    public float spreadAngle = 5f;
    //public ParticleSystem muzzleFlash;

    public Transform firePoint;

    public override void ResetRot()
    {
        rot = Vector3.Lerp(rot, Vector3.zero, spd * Time.deltaTime);
    }

    public override void Recoil()
    {
        rot += new Vector3(-mag, 0, 0f);
    }

    public override void Use()
    {
        if (!canFire || shells <= 0)
        {
            soundManager.ShotEmpty();
            return;
        }

        StartCoroutine(FireCooldown());
        Recoil();
        soundManager.ShotShot();
        //muzzleFlash?.Play();
        shells--;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 spread = new Vector3(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
            Quaternion rotation = Quaternion.Euler(player.cam.transform.eulerAngles + spread);
            Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
            Hitscan(ray, firePoint);
        }
    }
}

