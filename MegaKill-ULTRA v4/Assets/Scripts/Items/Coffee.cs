using System.Collections;
using UnityEngine;

public class Coffee : MonoBehaviour
{
    PlayerController player;
    SoundManager soundManager;
    UX ux;
    CamController cam;
    float charge = 5f;
    float cooldown = -0.5f;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        soundManager = FindObjectOfType<SoundManager>();
        ux = FindObjectOfType<UX>();
        cam = FindObjectOfType<CamController>();
    }

    public void Use()
    {
        if (Time.time - cooldown >= 0.5f)
        {
            if (charge > 0)
            {
                charge--;
                soundManager.Gulp();
                cam.Focus();
                ux.PopUp("FOCUS UP");
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
