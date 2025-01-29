using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI title;
    public TextMeshProUGUI creditsBox;
    public string[] lines;
    public float textSpeed;

    int index;
    bool started = false;
    bool waiting = false;

    public PlayerInput playerInput;

    public bool credits;

    void Start()
    {
        if (credits)
        {
            StartDialogue();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !started && !credits)
        {
            started = true;
            textComponent.text = string.Empty;
            creditsBox.text = string.Empty;
            title.text = string.Empty;
            StartDialogue();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && waiting)
        {
            waiting = false;
            playerInput.Clear();
            NextLine();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.C) && !started)
        {
            SceneManager.LoadScene(2);
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        if (lines[index].EndsWith("?"))
        {
            waiting = true;
            playerInput.InputBox();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            NextLine();
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(LoadNext());
        }
    }

    IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(1f);

        if (!credits)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
}
