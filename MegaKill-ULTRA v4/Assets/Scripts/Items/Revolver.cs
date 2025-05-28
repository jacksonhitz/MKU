using System.Collections;
using UnityEngine;


public class Revolver : Gun
{
   public override void Use()
   {
        if (!canFire) return;
        if (bullets > 0)
        {
            Debug.Log("FUCK OFF");

            StartCoroutine(FireCooldown());
            Recoil();
            soundManager.RevShot();
            data.muzzleFlash.Play();

            bullets--;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Hitscan(ray);

            Debug.Log("shot fired");
        }
        else
        {
            popUp.UpdatePopUp("EMPTY");
            soundManager.RevEmpty();
        } 
   }
}



