using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        gameManager.Tutorial();
        gameManager.CollectItems();
    }

    void WASD()
    {
        //wasd
    }
    void Jump()
    {
        //space
    }
    void PunchL()
    {
        // left click to punch
    }
    void PunchR()
    {
        // right click to punch
    }
    void Items()
    {
        // tab to see items
    }
    void PickupL()
    {
        // q to pickup to left hand
    }
    void PickupR()
    {
        // e to pickup to right hand
    }
    void CickL()
    {
        // left click to use left item
    }
    void ClickR()
    {
        //right click to use right item
    }
    void ThrowL()
    {
        // q to throw left
    }
    void ThrowR()
    {
        // e to throw right
    }
}
