using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Meth : MonoBehaviour
{
    BulletTime bulletTime;
    SoundManager soundManager;
    UIManager ui;

    public float charge = 100f;
    float cooldown = -0.5f;

    public bool left;

    void Awake()
    {
        bulletTime = FindObjectOfType<BulletTime>();
        soundManager = FindObjectOfType<SoundManager>();
        ui = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && left || (Input.GetKeyUp(KeyCode.Mouse0) && left))
        {
            bulletTime.Reg();
        }
    }

    public void Use()
    {
        if (charge > 0)
        {
            if (Input.GetKey(KeyCode.Mouse0) && left)
            {
                charge -= 0.01f;
                bulletTime.Slow();
            }
            else
            {
                bulletTime.Reg();
            }
            if (Time.time - cooldown >= 0.5f)
            {
                ui.PopUp("TIME SLOWED");
                soundManager.Gulp();
                cooldown = Time.time; 
            }
        }
        else
        {
            bulletTime.Reg();
            if (Time.time - cooldown >= 0.5f)
            {
                ui.PopUp("EMPTY");
                soundManager.PillEmpty();
                cooldown = Time.time; 
            }
        }
    }
}
