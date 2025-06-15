using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Command : MonoBehaviour
{
    TextMeshProUGUI text;
    Vector3 ogPos;

    void Awake()
    {
        ogPos = transform.localPosition;

        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Off();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;

        StateChange(StateManager.State);
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }

    void StateChange(StateManager.GameState state)
    {
        // TODO: Remove this while keeping the effect
        switch (state)
        {
            case StateManager.GameState.REHEARSAL:
                On();
                break;
            // case StateManager.GameState.TANGO2: On(); break;
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
        transform.localPosition = ogPos;
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
            transform.localPosition = ogPos + randomOffset;

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

        transform.localPosition = ogPos;
        text.enabled = true;
        Off();
    }
}
