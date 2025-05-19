using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    public string[] lines;
    public float textSpeed;

    int index = -1;
    bool customTyping = false;

    SoundManager soundManager;

    void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
      //  if (Input.GetKeyDown(KeyCode.Space) && !started)
      //  {
      //     started = true;
      //     ClearText();
      //     NextLine();
      //  }
    }

    public void Play()
    {
        StartCoroutine(DelayDialogue());
    }

    IEnumerator DelayDialogue()
    {
        yield return new WaitForSeconds(1.5f);
        NextLine();
    }

    public void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine(lines[index]));
        }
        else
        {
            StartCoroutine(Done());
        }
    }

    IEnumerator TypeLine(string text)
    {
        
        textComponent.text = string.Empty;
        yield return new WaitForSeconds(0.1f);

        soundManager?.NewLine();

        foreach (char c in text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(1f);

        soundManager?.StopLine();

        if (customTyping)
        {
            customTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    public void TypeText(string customText)
    {
        StopAllCoroutines();
        customTyping = true;
        StartCoroutine(TypeLine(customText));
    }

    public void Off()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
    }

    IEnumerator Done()
    {
        yield return new WaitForSeconds(2f);

        textComponent.text = string.Empty;

        if (StateManager.IsPassive())
        {
            StateManager.NextState();
            Debug.Log("nextState");
        }
    }
}
