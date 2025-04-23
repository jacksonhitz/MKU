using System.Collections;
using UnityEngine;


public class Revolver : Gun
{
   public override void Use()
   {
        if (!canFire) return;
        if (bullets > 0)
        {
            StartCoroutine(FireCooldown());
            Recoil();
            soundManager.RevShot();
            gunData.muzzleFlash.Play();

            bullets--;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Hitscan(ray);
        }
        else
        {
            soundManager.RevEmpty();
        } 
   }
}



