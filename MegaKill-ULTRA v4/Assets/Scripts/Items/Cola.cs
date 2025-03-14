using System.Collections;
using UnityEngine;

public class Cola : MonoBehaviour
{
    PlayerController player;
    SoundManager soundManager;
    UX ux;
    float charge = 5f;
    float cooldown = -0.5f;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        soundManager = FindObjectOfType<SoundManager>();
        ux = FindObjectOfType<UX>();
    }

    public void Use()
    {
        if (Time.time - cooldown >= 0.5f)
        {
            if (charge > 0)
            {
                charge--;
                soundManager.Gulp();
                player.SpeedUp();
                ux.PopUp("SPEED UP");
                cooldown = Time.time; 
            }
            else
            {
                soundManager.PillEmpty();
                ux.PopUp("EMPTY");
            }
        }
    }
}
