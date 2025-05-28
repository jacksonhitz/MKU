using UnityEngine;

public class Consumable : Item
{
    [SerializeField] ConsumableData data;

    float charge;

    public override void Start()
    {
        base.Start();
        itemData = data;
        charge = data.charge;
    }

    public override void Use()
    {
        base.Use();
        if (charge > 0 && holder is PlayerController player)
        {
            charge--;
            player.health.Heal(data.heal);
            //trip

            SoundManager.Instance.Gulp();
            popUp.UpdatePopUp("HEALTH UP");
        }
        else
        {
            popUp.UpdatePopUp("EMPTY");
            SoundManager.Instance.PillEmpty();
        }
    }
}
