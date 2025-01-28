using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class AmmoDisplay : MonoBehaviour { 
    public int ammo;
    public int maxAmmo = 6;
    public bool isFiring;
    public TextMeshProUGUI ammoDisplay;
    // Start is called before the first frame update
    void Start() {
        ammo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        ammoDisplay.text = ammo.ToString();
        if(Input.GetMouseButtonDown(0) && !isFiring && ammo > 0)
        {
            isFiring = true;
            ammo--;
            isFiring = false; 
        }

         if(Input.GetKeyDown(KeyCode.R) && ammo < maxAmmo)
        {
            ammo = maxAmmo;
        }
        
    }
}

