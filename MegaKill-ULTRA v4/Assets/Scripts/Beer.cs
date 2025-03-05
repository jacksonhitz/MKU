using System.Collections;
using UnityEngine;

public class Beer : MonoBehaviour
{
    PlayerController player;
    SoundManager soundManager;
    UX ux;
    float charge = 0f;
    float heal = 4f;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        soundManager = FindObjectOfType<SoundManager>();
        ux = FindObjectOfType<UX>();
    }

    public void Use()
    {
        if (charge > 0)
        {
            charge--;
            soundManager.Gulp();
            player.Heal();
            ux.PopUp("HEALTH UP");
        }
        else
        {
            //soundManager.PillEmpty();
        }
    }
}
