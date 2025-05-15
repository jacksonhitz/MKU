using System.Collections;
using UnityEngine;

public class Revolver : Gun
{
    float targetAdjust = 0.25f;

    public override void Use()
    {
        Vector3 dir; 
        if (currentState == ItemState.Player)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            dir = ray.direction;
            if (bullets > 0)
            {
                bullets--;
                FireBasic();
                FireRay(dir);

                sound?.RevShot();
            }
            else
            {
                popUp?.UpdatePopUp("EMPTY");
                sound?.RevEmpty();
            }
        }
        else if (currentState == ItemState.Enemy && holder is Enemy enemy)
        {
            Vector3 target = enemy.target.transform.position;
            target.y += targetAdjust;
            dir = (target - firePoint.position).normalized;

            FireBasic();
            FireBullet(dir);

            //sound?.EnemySFX(enemy.sfx, enemy.attackClip);
        }
    }

    void FireBasic()
    {
        Recoil();
        data.muzzleFlash?.Play();
    }

}
