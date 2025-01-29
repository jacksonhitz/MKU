using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class AmmoDisplay : MonoBehaviour 
{ 

    public int maxAmmo = 6;
    public bool isFiring;
    public TextMeshProUGUI ammoDisplay;

    PlayerController player;
    
    void Start() 
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    void Ammo()
    {
        if (player.weapon == 1)
        {
            
        }
        
        ammoDisplay.text = player.gun.bullets.ToString();
    }
}

