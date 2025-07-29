using UnityEngine;

public class MG : Gun
{
    float targetAdjust = 0.25f;
    private bool isFiring;

    public override void Use()
    {
        Vector3 dir = camera.transform.forward; // default fallback direction

        if (currentState == ItemState.Player)
        {
            isFiring = true;
            if (bullets > 0)
            {
                bullets--;

                Vector3 spread = new Vector3(
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    0f
                );
                Quaternion rotation = Quaternion.Euler(camera.transform.eulerAngles + spread);
                Ray ray = new Ray(firePoint.position, rotation * Vector3.forward);
                dir = ray.direction;

                FireVFX();
                FireRecoil();
                FireRay(dir);

                sound.CreateSoundBuilder().Play("MGShot");
            }
            else
            {
                popUp?.UpdatePopUp("EMPTY");
                sound.CreateSoundBuilder().Play("MGEmpty");
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

            FireVFX();
            FireBullet(dir);

            sound.CreateSoundBuilder().WithPosition(enemy.transform.position).Play("MGShot");
        }
    }

    private new void Update()
    {
        base.Update();
        if (!isFiring || currentState is not ItemState.Player || bullets <= 0)
            return;
        if (
            PlayerController.Instance.items.leftItem == this
                && InputManager.PlayerActionMap.UseLeft.IsPressed()
            || PlayerController.Instance.items.rightItem == this
                && InputManager.PlayerActionMap.UseRight.IsPressed()
        )
        {
            UseCheck();
        }
        else
        {
            isFiring = false;
        }
    }
}
