using System.Collections;
using UnityEngine;

public class Beer : MonoBehaviour
{
    PlayerController player;
    SoundManager soundManager;
    UIManager ui;
    float charge = 5f;
    float cooldown = -0.5f;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        soundManager = FindObjectOfType<SoundManager>();
        ui = FindObjectOfType<UIManager>();
    }

    public void Use()
    {
        if (Time.time - cooldown >= 0.5f)
        {
            if (charge > 0)
            {
                charge--;
                soundManager.Gulp();
                player.Heal(10);
                ui.PopUp("HEALTH UP");
                cooldown = Time.time; 
            }
            else
            {
                soundManager.PillEmpty();
                ui.PopUp("EMPTY");
            }
        }
    }
}
