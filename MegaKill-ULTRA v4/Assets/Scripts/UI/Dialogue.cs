using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subheadText;
    public string[] lines;
    public float textSpeed;

    int index;
    bool started = false;

    GameManager gameManager;
    SoundManager soundManager;

    public bool title;
    public bool tutorial;

    bool intro;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void CallDialogue()
    {
        StartCoroutine(DelayDialogue());
    }
    IEnumerator DelayDialogue()
    {
        yield return new WaitForSeconds(1.5f);
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !started && title)
        {
            started = true;
            textComponent.text = string.Empty;
            titleText.text = string.Empty;
            subheadText.text = string.Empty;
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        yield return new WaitForSeconds(.1f);
        
        if (intro)
        {
            soundManager.NewLine();
        }
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(1f);
        if (intro)
        {
            //soundManager.StopLine();
        }
        NextLine();
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(Done());
        }
    }

    public void Off()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
    }

    IEnumerator Done()
    {
        yield return new WaitForSeconds(1f);

        if (StateManager.State == StateManager.GameState.Intro)
        {
            textComponent.text = string.Empty;
            gameManager.Tutorial();
        }
        else if (StateManager.State == StateManager.GameState.Tutorial)
        {
            textComponent.text = string.Empty;
            gameManager.Lvl();
        }
        else if (StateManager.State == StateManager.GameState.Title)
        {
            yield return new WaitForSeconds(2f);
            gameManager.Intro();
        }
    }
}
