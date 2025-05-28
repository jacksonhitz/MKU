using UnityEngine;

public class MG : Gun
{
    float cooldown = -0.15f;

    public override void Use()
    {
        if (!canFire) return;
        if (bullets > 0)
        {
            StartCoroutine(FireCooldown());
            Recoil();
            soundManager.MGShot();
            data.muzzleFlash.Play();
            bullets--;

            Vector3 spread = new Vector3(Random.Range(-data.spreadAngle, data.spreadAngle), Random.Range(-data.spreadAngle, data.spreadAngle), 0f);
            Quaternion rotation = Quaternion.Euler(playerController.cam.transform.eulerAngles + spread);
            Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
            Hitscan(ray);
        }
        else
        {
            if (Time.time - cooldown >= 0.5f)
            {
                popUp.UpdatePopUp("EMPTY");
                soundManager.MGEmpty();
                cooldown = Time.time;
            }
        }
    }
}
