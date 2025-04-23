using System.Collections;
using UnityEngine;

public class Shotgun : Gun
{ 
    public override void Use()
    {
        if (!canFire) return;
        if (bullets > 0)
        {
            Recoil();
            soundManager.ShotShot();
            data.muzzleFlash?.Play();
            bullets--;
            for (float i = 0; i < data.pellets; i++)
            {
                Vector3 spread = new Vector3(Random.Range(-data.spreadAngle, data.spreadAngle), Random.Range(-data.spreadAngle, data.spreadAngle), 0f);
                Quaternion rotation = Quaternion.Euler(playerController.cam.transform.eulerAngles + spread);
                Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
                Hitscan(ray);
            }
        }
        else
        {
            popUp.UpdatePopUp("EMPTY");
            soundManager.ShotEmpty();
        }
    }

}
