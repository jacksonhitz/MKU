using System.Collections;
using UnityEngine;
public class Revolver : Gun
{
    public float bullets = 6f;
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
        if (!canFire || bullets <= 0)
        {
            soundManager.RevEmpty();
            return;
        }

        StartCoroutine(FireCooldown());
        Recoil();
        soundManager.RevShot();
        //muzzleFlash?.Play();
        bullets--;

        Ray ray = player.cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Hitscan(ray, firePoint);
    }
}