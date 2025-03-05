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

    public bool title;
    public bool intro;
    public bool tutorial;
    public bool passed;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();

        passed = true;
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            textComponent.text = "";
            StopAllCoroutines();

            if (title)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                gameManager.StartLvl();
            }
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
        if (index < lines.Length - 1 && passed)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(Done());
        }
    }

    IEnumerator Done()
    {
        yield return new WaitForSeconds(1f);

        if (intro)
        {
            textComponent.text = string.Empty;
            gameManager.EndIntro();
        }
        else if (tutorial)
        {
            textComponent.text = string.Empty;
            gameManager.StartLvl();
        }
        else
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(1);
        }
    }
}
