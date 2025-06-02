using UnityEngine;

public class Shotgun : Gun
{
    float targetAdjust = 0.25f;

    public override void Use()
    {
        if (currentState == ItemState.Player)
        {
            if (bullets > 0)
            {
                bullets--;

                for (int i = 0; i < data.pellets; i++)
                {
                    Vector3 dir = Camera.main.transform.forward; 

                    Vector3 spread = new Vector3(
                        Random.Range(-data.spreadAngle, data.spreadAngle),
                        Random.Range(-data.spreadAngle, data.spreadAngle),
                        0f
                    );
                    Quaternion rotation = Quaternion.Euler(Camera.main.transform.eulerAngles + spread);
                    Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
                    dir = ray.direction;

                    FireVFX();
                    FireRay(dir);

                    sound.Play("SGShot");
                }
            }
            else
            {
                popUp?.UpdatePopUp("EMPTY");
                sound.Play("SGShot");
            }
        }
        else if (currentState == ItemState.Enemy && holder is Enemy enemy)
        {
            Vector3 target = enemy.target.transform.position;
            target.y += targetAdjust;

            for (int i = 0; i < data.pellets; i++)
            {
                // Apply random spread
                Vector3 spread = new Vector3(
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    0f
                );
                Quaternion rotation = Quaternion.Euler(spread);
                Vector3 dir = rotation * (target - firePoint.position).normalized;

                enemy.CallUse();

                FireVFX();
                FireBullet(dir);

                sound.Play("SGShot", enemy.transform.position);
            }
        }
    }
}
