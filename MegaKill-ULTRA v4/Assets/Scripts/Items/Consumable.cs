using UnityEngine;

public class Consumable : Item
{
    [SerializeField] ConsumableData data;

    float charge;

    protected override void Start()
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

            sound.Play("Gulp");
            popUp.UpdatePopUp("HEALTH UP");
        }
        else
        {
            popUp.UpdatePopUp("EMPTY");
            sound.Play("PillEmpty");
        }
    }
}
