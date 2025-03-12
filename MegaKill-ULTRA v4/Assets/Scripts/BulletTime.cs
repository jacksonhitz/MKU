using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour
{
    float slowSpd = 0.05f; 
    float transitionDuration = 0.5f; 
    float originalSpd;
    public bool isSlow;

    SoundManager soundManager;
    GameManager gameManager;

    void Start()
    {
        originalSpd = Time.timeScale;
        soundManager = FindAnyObjectByType<SoundManager>();
        gameManager = FindAnyObjectByType<GameManager>();
    }
    public void Slow()
    {
        isSlow = true;
        soundManager.SetSpeed(SoundManager.GameSpeed.Slow);
        StartCoroutine(LerpTime(slowSpd));
    }

    public void Reg()
    {
        isSlow = true;
        soundManager.SetSpeed(SoundManager.GameSpeed.Regular);
        StartCoroutine(LerpTime(originalSpd));
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
