using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Command : MonoBehaviour
{
    GameObject obj;
    TextMeshProUGUI text;
    Vector3 originalPosition;

    void Awake()
    {
        obj = this.gameObject;
        originalPosition = obj.transform.localPosition;

        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Off();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }

    void StateChange(StateManager.GameState state)
    {
        if (StateManager.State == StateManager.GameState.Fight)
        {
            On();
        }
        else
        {
            Off();
        }
    }

    void On()
    {
        text.enabled = true;
        StartCoroutine(Jitter());
    }

    void Off()
    {
        text.enabled = false;
        obj.transform.localPosition = originalPosition;
    }

    IEnumerator Jitter()
    {
        float timer = 0f;
        float jitterAmount = 10f;
        float flashInterval = 0.1f;
        float flashTimer = 0f;
        bool textVisible = true;

        while (timer < 2f)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount),
                Random.Range(-jitterAmount, jitterAmount)
            );
            obj.transform.localPosition = originalPosition + randomOffset;

            flashTimer += Time.deltaTime;
            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                textVisible = !textVisible;
                text.enabled = textVisible;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        obj.transform.localPosition = originalPosition;
        text.enabled = true;
        Off();
    }
}
