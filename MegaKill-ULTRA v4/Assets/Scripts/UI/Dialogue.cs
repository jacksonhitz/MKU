using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public static Dialogue Instance { get; set; }

    public TextMeshProUGUI textComponent;

    public string[] lines;
    public float textSpeed;

    int index = -1;
    bool customTyping = false;


    void Awake()
    {
        Instance = this;
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
            Invoke("Done", 2f);
        }
    }

    IEnumerator TypeLine(string text)
    {
        
        textComponent.text = string.Empty;
        yield return new WaitForSeconds(0.1f);

        SoundManager.Instance.Play("Line");

        foreach (char c in text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(1f);

        SoundManager.Instance.Stop();

        if (customTyping)
        {
            customTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    public void TypeText(string customText, float timer)
    {
        StopAllCoroutines();
        customTyping = true;
        StartCoroutine(TypeLine(customText));

        if (timer != 0)
            Invoke("Done", timer);
    }

    public void Off()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
    }

    void Done()
    {
        textComponent.text = string.Empty;

        if (StateManager.State == StateManager.GameState.FILE)
            StateManager.StartLvl();
        else if (StateManager.IsPassive())
            StateManager.LoadNext();
    }
}
