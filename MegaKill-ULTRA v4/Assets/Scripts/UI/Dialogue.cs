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

    int index = -1;
    bool started = false;

    GameManager gameManager;
    SoundManager soundManager;


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
        NextLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !started)
        {
            started = true;
            textComponent.text = string.Empty;
            titleText.text = string.Empty;
            subheadText.text = string.Empty;
            NextLine();
        }
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
        if (state != StateManager.GameState.Paused)
        {

        }
        else
        {

        }
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

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;
        yield return new WaitForSeconds(.1f);
        
        if (StateManager.State == StateManager.GameState.Intro)
        {
            soundManager.NewLine();
        }
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(1f);
        if (StateManager.State == StateManager.GameState.Testing)
        {
            soundManager.StopLine();
        }
        NextLine();
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
        textComponent.text = "";
    }
}
