using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour
{
    public float slowSpd = 0.05f; 
    public float transitionDuration = 0.5f; 
    float originalSpd;
    bool isSlowed = false;
    bool wasSlowed = false;

    SoundManager soundManager;

    UX ux;

    void Start()
    {
        originalSpd = Time.timeScale;
        ux = FindObjectOfType<UX>(); 
        soundManager = FindAnyObjectByType<SoundManager>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isSlowed)
                Slow();

                if (!wasSlowed)
                {
                    ux.Arms();
                }
        }
        else
        {
            if (isSlowed)
                Reg();
        }
    }

    public void Slow()
    {
        isSlowed = true;
        StartCoroutine(LerpTime(slowSpd));

        soundManager.Slow();

    }

    public void Reg()
    {
        isSlowed = false;
        StartCoroutine(LerpTime(originalSpd));
        
        soundManager.Reg();
    }

    IEnumerator LerpTime(float targetTimeScale)
    {
        float startScale = Time.timeScale;
        float t = 0f;

        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, t / transitionDuration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale; 
            yield return null;
        }

        Time.timeScale = targetTimeScale;
    }
}
