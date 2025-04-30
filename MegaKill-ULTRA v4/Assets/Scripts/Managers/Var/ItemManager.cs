using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    List<Item> items;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        items = new List<Item>();
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
