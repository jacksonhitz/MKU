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
    bool waiting = true;
    GameManager gameManager;
    SoundManager soundManager;
    SceneLoader sceneLoader;

    public bool title;
    public bool intro;
    public bool tutorial;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        sceneLoader = FindObjectOfType<SceneLoader>();
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
        waiting = true;
        
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
            soundManager.StopLine();
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

        if (StateManager.state == StateManager.GameState.Intro)
        {
            textComponent.text = string.Empty;
            gameManager.EndIntro();
        }
        else if (StateManager.state == StateManager.GameState.Lvl)
        {
            textComponent.text = string.Empty;
            gameManager.EndTutorial();
        }
        else if (StateManager.state == StateManager.GameState.Title)
        {
            yield return new WaitForSeconds(2f);
            StateManager.state = StateManager.GameState.Intro;
            sceneLoader.Lvl();
        }
    }
}
