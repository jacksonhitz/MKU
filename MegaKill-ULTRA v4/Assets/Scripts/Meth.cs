using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meth : MonoBehaviour
{
    BulletTime bulletTime;
    public float charge = 100f;

    void Awake()
    {
        bulletTime = FindAnyObjectByType<BulletTime>();
    }

    void Use()
    {
        if (charge > 0)
        {
            bulletTime.Slow();
        }
        else
        {
            bulletTime.Reg();
        }
    }
}
