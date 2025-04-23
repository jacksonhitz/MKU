using UnityEngine;

public class Consumable : Item
{
    [SerializeField] ConsumableData data;

    public override void Use()
    {
        if (Time.time - data.cooldown >= 0.5f)
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
                soundManager.PillEmpty();
                popUp.UpdatePopUp("EMPTY"); 
            }
        }
    }
}
