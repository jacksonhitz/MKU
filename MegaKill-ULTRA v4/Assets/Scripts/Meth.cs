using System.Collections;
using UnityEngine;

public class Meth : MonoBehaviour
{
    BulletTime bulletTime;
    SoundManager soundManager;
    UX ux;

    public float charge = 10f;
    public float slowDuration = 1f;

    bool isSlow;

    void Awake()
    {
        bulletTime = FindObjectOfType<BulletTime>();
        soundManager = FindObjectOfType<SoundManager>();
        ux = FindObjectOfType<UX>();
    }

    public void Use()
    {
        if (charge > 0 && !isSlow)
        {
            soundManager.Gulp();
            //ux.PopUp("TIME SLOWED");
            StartCoroutine(SlowRoutine());
        }
        if (charge == 0)
        {
            soundManager.PillEmpty();
        }
    }

    IEnumerator SlowRoutine()
    {
        isSlow = true;
        bulletTime.Slow();
        yield return new WaitForSeconds(slowDuration);
        bulletTime.Reg();
        isSlow = false;
    }
}
