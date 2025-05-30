using UnityEngine;

public class MG : Gun
{
    float targetAdjust = 0.25f;

    public override void Use()
    {
        Vector3 dir = Camera.main.transform.forward; // default fallback direction

        if (currentState == ItemState.Player)
        {
            if (bullets > 0)
            {
                bullets--;

                Vector3 spread = new Vector3(
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    0f
                );
                Quaternion rotation = Quaternion.Euler(Camera.main.transform.eulerAngles + spread);
                Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
                dir = ray.direction;

                FireBasic();
                FireRay(dir);

                sound.Play("MGShot");
            }
            else
            {
                popUp?.UpdatePopUp("EMPTY");
                sound.Play("MGEmpty");
            }
        }
        else if (currentState == ItemState.Enemy && holder is Enemy enemy)
        {
            Vector3 target = enemy.target.transform.position;
            target.y += targetAdjust;

            Vector3 spread = new Vector3(
                Random.Range(-data.spreadAngle, data.spreadAngle),
                Random.Range(-data.spreadAngle, data.spreadAngle),
                0f
            );
            Quaternion rotation = Quaternion.Euler(spread);
            dir = rotation * (target - firePoint.position).normalized;

            enemy.CallUse();

            FireBasic();
            FireBullet(dir);

            sound.Play("MGShot", enemy.transform.position);
        }
    }

    void FireBasic()
    {
        Recoil();
        muzzleFlash.Play();
    }
}
