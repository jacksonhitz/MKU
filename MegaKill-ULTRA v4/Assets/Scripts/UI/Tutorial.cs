using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
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

    public void States(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.WASD:
                Debug.Log("Tutorial: WASD");
                break;
            case State.Jump:
                Debug.Log("Tutorial: Jump");
                break;
            case State.Kill:
                Debug.Log("Tutorial: Kill");
                break;
            case State.Grab:
                Debug.Log("Tutorial: Grab");
                break;
            case State.Throw:
                Debug.Log("Tutorial: Throw");
                break;
            case State.Slow:
                Debug.Log("Tutorial: Slow");
                break;
            case State.Off:
                Debug.Log("Tutorial: Off");
                break;
            default:
                break;
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
        States(State.WASD);
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
            gameManager.StartLvl();
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
        soundManager.StopLine();
        waiting = true;
    }
}
