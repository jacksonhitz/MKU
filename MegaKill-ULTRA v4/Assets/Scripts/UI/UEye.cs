using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UEye : MonoBehaviour
{
    [SerializeField] RawImage currentEye;

    [SerializeField] Texture eye100Texture;
    [SerializeField] Texture eye80Texture;
    [SerializeField] Texture eye75Texture;
    [SerializeField] Texture eye60Texture;
    [SerializeField] Texture eye50Texture;
    [SerializeField] Texture eye40Texture;
    [SerializeField] Texture eye25Texture;
    [SerializeField] Texture eye20Texture;
    [SerializeField] Texture eye10Texture;
    [SerializeField] Texture eyeFlashTexture;

    [SerializeField] Texture eye100Texture2;
    [SerializeField] Texture eye80Texture2;
    [SerializeField] Texture eye75Texture2;
    [SerializeField] Texture eye60Texture2;
    [SerializeField] Texture eye50Texture2;
    [SerializeField] Texture eye40Texture2;
    [SerializeField] Texture eye25Texture2;
    [SerializeField] Texture eye20Texture2;
    [SerializeField] Texture eye10Texture2;
    [SerializeField] Texture eyeFlashTexture2;

    [SerializeField] Texture eyeHurtTexture;
    [SerializeField] Texture eyeHealTexture;

    float health = 100f;
    bool isFlashing = false;
    Coroutine eyeIdleCoroutine;
    bool useAlternateTexture = false;
    float eyeIdleAnimationDuration = 1.5f;

    void OnEnable()
    {
        StateManager.OnStateChanged += OnStateChanged;
        StateManager.OnSilentChanged += OnStateChanged;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= OnStateChanged;
        StateManager.OnSilentChanged -= OnStateChanged;
    }
    void OnStateChanged(StateManager.GameState state)
    {
        Debug.Log("state changed");
        if (StateManager.IsActive())
        {
            On();
        }
        else
        {
            Off();
        }
    }

    public void On()
    {
        currentEye.enabled = true;
    }
    void Off()
    {
        currentEye.enabled = false;
    }

    public void UpdateHealth(float newHealth)
    {
        float oldHealth = health;
        health = newHealth;
        
        StopAllCoroutines();

        if (newHealth >= oldHealth)
        {
            StartCoroutine(HealEye());
        }
        else
        {
            StartCoroutine(HurtEye());
        }
    }

    IEnumerator HurtEye()
    {
        Texture previousEye = currentEye.texture;
        currentEye.texture = eyeHurtTexture;
        //number.SetActive(false)
        yield return new WaitForSeconds(0.2f);
        currentEye.texture = previousEye;

        if (health > 10)
        {
            currentEye.texture = GetEye();
            eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
           
        }
        else
        {
            StartCoroutine(FlashEye());
        }
    }

    IEnumerator HealEye()
    {
        Texture previousEye = currentEye.texture;
        currentEye.texture = eyeHealTexture;
        //number.SetActive(false)
        yield return new WaitForSeconds(0.2f);
        currentEye.texture = previousEye;
        currentEye.texture = GetEye(); //cut maybe

        eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
    }

    IEnumerator FlashEye()
    {
        Texture[] flashSequence = new Texture[]
        {
            eye10Texture,
            eye10Texture2,
            eyeFlashTexture,
            eyeFlashTexture2
        };

        int currentIndex = 0;

        while (health <= 10)
        {
            currentEye.texture = flashSequence[currentIndex];
            currentIndex = (currentIndex + 1) % flashSequence.Length;

            yield return new WaitForSeconds(0.2f);
        }
        currentEye.texture = GetEye(); //cut maybe
        

        eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
    }

    IEnumerator IdleEyeAnimation()
    {
        while (true)
        {
            useAlternateTexture = !useAlternateTexture;
            currentEye.texture = GetEye();
            //number.SetActive(true) or enable
            yield return new WaitForSeconds(eyeIdleAnimationDuration);
        }
    }

    Texture GetEye()
    {
        if (!useAlternateTexture)
        {
            if (health > 80) return eye100Texture;
            if (health > 60) return eye80Texture;
            if (health > 40) return eye60Texture;
            if (health > 20) return eye40Texture;
            if (health > 10) return eye20Texture;
            return eye10Texture;
        }
        else
        {
            if (health > 80) return eye100Texture2;
            if (health > 60) return eye80Texture2;
            if (health > 40) return eye60Texture2;
            if (health > 20) return eye40Texture2;
            if (health > 10) return eye20Texture2;
            return eye10Texture2;
        }
    }

    public void StartIdleAnimation()
    {
        eyeIdleCoroutine = StartCoroutine(IdleEyeAnimation());
    }
}
