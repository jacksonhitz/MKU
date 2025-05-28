using UnityEngine;

public class Consumable : Item
{
    [SerializeField] ConsumableData data;

    float cooldown = -0.15f;
    float charge;

    public override void Start()
    {
        base.Start();
        charge = data.charge;

    }

    public override void Use()
    {
        if (charge > 0)
        {
            charge--;
            playerController.Heal(data.heal);
            //trip
            cooldown = Time.time;

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
