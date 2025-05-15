using UnityEngine;

public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Cons,
        Gun,
    }

    public string itemName;
    public ItemType type;
    public GameObject prefab;
    public float rate;
    public Vector3 pos;
    public Vector3 rot;
}
