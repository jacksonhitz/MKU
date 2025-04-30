using UnityEngine;

public class Consumable : Item
{
    [SerializeField] ConsumableData data;

    float cooldown = -0.15f;

    public override void Use()
    {
        if (data.charge > 0)
        {
            data.charge--;
            playerController.Heal(data.heal);
            //trip
            data.cooldown = Time.time;

            soundManager.Gulp();
            popUp.UpdatePopUp("HEALTH UP");
        }
        else
        {

            if (Time.time - cooldown >= 0.5f)
            {
                popUp.UpdatePopUp("EMPTY");
                soundManager.PillEmpty();
                cooldown = Time.time;
            }
        }
    }
}
