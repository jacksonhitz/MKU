using UnityEngine;

public class ItemData : ScriptableObject
{
    public enum ItemState
    {
        Available,
        Enemy,
        Player
    }

    public enum ItemType
    {
        Cons,
        Gun,
    }


    public string itemName;
    public ItemType type;
    public ItemState state;
    public GameObject prefab;
}
