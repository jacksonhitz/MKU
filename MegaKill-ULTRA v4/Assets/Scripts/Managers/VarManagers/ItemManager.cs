using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] List<Item> items;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        items = new List<Item>();
        CollectItems();
    }
   
    public void CollectItems()
    {
        items.Clear();
        items.AddRange(FindObjectsOfType<Item>());
    }

    public void HighlightAll()
    {
        foreach (Item item in items)
        {
            item.GlowMat();
        }
    }
    public void HighlightItem()
    {
        foreach (Item item in items)
        {
            if (item.isHovering && item.currentState == Item.ItemState.Available)
            {
                item.GlowMat();
            }
            else
            {
                item.DefaultMat();
            }
        }
    }
}
