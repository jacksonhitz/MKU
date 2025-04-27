using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    public GameObject titleScreen;

    public string[] lines;
    public float textSpeed;

    int index = -1;
    bool started = false;
    bool customTyping = false;

    GameManager gameManager;
    SoundManager soundManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }

    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
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

    public void CallDialogue()
    {
        StartCoroutine(DelayDialogue());
    }

    IEnumerator DelayDialogue()
    {
        yield return new WaitForSeconds(1.5f);
        NextLine();
    }

    void StateChange(StateManager.GameState state)
    {
        
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
        Debug.Log("typing");
        
        textComponent.text = string.Empty;
        yield return new WaitForSeconds(0.1f);

      //  soundManager.NewLine();

        foreach (char c in text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(1f);

     //   soundManager.StopLine();

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
        ClearText();
    }

    void ClearText()
    {
        textComponent.text = string.Empty;

        if (titleScreen != null)
        {
            titleScreen.SetActive(false);
        }
    }

    IEnumerator Done()
    {
        yield return new WaitForSeconds(2f);

        if (StateManager.State == StateManager.GameState.Intro)
        {
            textComponent.text = string.Empty;
            gameManager.Tutorial();
        }
        else if (StateManager.State == StateManager.GameState.Tutorial)
        {
            textComponent.text = string.Empty;
            gameManager.Launch();
        }
        else if (StateManager.State == StateManager.GameState.Title)
        {
            yield return new WaitForSeconds(2f);
            gameManager.Intro();
        }
    }
}
