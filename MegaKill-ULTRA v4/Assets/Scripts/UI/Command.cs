using System.Collections;
using System.Collections.Generic;
using Redcode.Moroutines;
using TMPro;
using UnityEngine;

public class Command : MonoBehaviour
{
    public bool Active
    {
        get => text.enabled;
        set
        {
            if (value)
            {
                On();
            }
            else
            {
                Off();
            }
        }
    }
    TextMeshProUGUI text;
    Vector3 ogPos;
    private Moroutine _jitter;

    void Awake()
    {
        ogPos = transform.localPosition;

        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        Off();
        _jitter = Moroutine.Create(gameObject, Jitter());
    }

    public void On()
    {
        text.enabled = true;
        _jitter.Rerun();
    }

    public void Off()
    {
        text.enabled = false;
        transform.localPosition = ogPos;
        _jitter?.Stop();
    }

    private IEnumerable Jitter()
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
