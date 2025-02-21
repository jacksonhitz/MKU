using System.Collections;
using UnityEngine;

public class Meth : MonoBehaviour
{
    BulletTime bulletTime;

    public float charge = 10f;
    public float slowDuration = 3f;

    bool isSlow;

    void Awake()
    {
        bulletTime = FindAnyObjectByType<BulletTime>();
    }

    public void Use()
    {
        if (charge > 0 && !isSlow)
        {
            StartCoroutine(SlowRoutine());
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
