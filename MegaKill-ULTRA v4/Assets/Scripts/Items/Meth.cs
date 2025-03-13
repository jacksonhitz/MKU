using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Meth : MonoBehaviour
{
    BulletTime bulletTime;
    SoundManager soundManager;
    UX ux;

    public float charge = 100f;
    float cooldown = -0.5f;

    public bool left;

    void Awake()
    {
        bulletTime = FindObjectOfType<BulletTime>();
        soundManager = FindObjectOfType<SoundManager>();
        ux = FindObjectOfType<UX>();
    }

    void Update()
    {
        if (left && Input.GetKeyUp(KeyCode.Mouse0) || !left && Input.GetKeyUp(KeyCode.Mouse1) || charge <= 0)
        {
            bulletTime.Reg();
        }
    }

    public void Use()
    {
        if (charge > 0)
        {
            if (Time.time - cooldown >= 0.5f)
            {
                ux.PopUp("TIME SLOWED");
                soundManager.Gulp();
                cooldown = Time.time; 
            }

            charge -= 0.01f;
            bulletTime.Slow();
        }
        else
        {
            if (Time.time - cooldown >= 0.5f)
            {
                ux.PopUp("EMPTY");
                soundManager.PillEmpty();
                cooldown = Time.time; 
            }
        }
    }
}
