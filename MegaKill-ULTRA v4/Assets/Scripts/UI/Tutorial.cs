using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string[] lines;
    public float textSpeed;

    public int index;
    GameManager gameManager;
    SoundManager soundManager;
    public bool hasMoved;
    public bool hasJumped;
    public bool hasClicked;
    public bool hasGrabbed;
    public bool hasThrown;
    public bool hasSlowed;

    public enum State
    {
        None,
        WASD,
        Jump,
        Slow,
        Grab,
        Kill,
        Throw,
        Off
    }

    public bool passed;
    public bool waiting;

    public State currentState;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();

    }

    void Update()
    {
        if (waiting)
        {
            waiting = false;
            NextLine();
        }
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
    public void StartDialogue()
    {
        index = 0;
        text.text = "";
        StartCoroutine(TypeLine());
    }

    public void CallOff()
    {
        StartCoroutine(Off());
    }

    IEnumerator Off()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(1f);
        text.text = "";
    }
    public void NextLine()
    {
        text.text = "";
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameManager.Lvl();
        }
    }
    IEnumerator TypeLine()
    {
        soundManager.NewLine();
        
        foreach (char c in lines[index].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(4f);
        waiting = true;
    }
}
