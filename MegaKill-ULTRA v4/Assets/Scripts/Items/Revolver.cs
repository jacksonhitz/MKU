using System.Collections;
using UnityEngine;

public class Revolver : Gun
{
    float targetAdjust = 0.25f;

    public override void Use()
    {
        Vector3 dir = Camera.main.transform.forward; 

        if (currentState == ItemState.Player)
        {
            if (bullets > 0)
            {
                bullets--;

                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f))
                    dir = (hitInfo.point - firePoint.position).normalized;

                FireBasic();
                FireRay(dir);

                sound.RevShot();
            }
            else
            {
                popUp?.UpdatePopUp("EMPTY");
                sound.RevEmpty();
            }
        }
        else if (currentState == ItemState.Enemy && holder is Enemy enemy)
        {
            Vector3 target = enemy.target.transform.position;
            target.y += targetAdjust;
            dir = (target - firePoint.position).normalized;

            enemy.CallUse();

            FireBasic();
            FireBullet(dir);

            sound.EnemySFX(enemy.sfx, enemy.attackClip);
        }
    }


    void FireBasic()
    {
        Recoil();
        muzzleFlash.Play();
    }

}
