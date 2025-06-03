using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable")]
public class ConsumableData : ItemData
{
    public float charge;
    public float heal;
    public float trip;
}
