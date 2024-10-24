using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI title;
    public string[] lines;
    public float textSpeed;

    int index;
    bool started = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!started)
            {
                started = true;
            
                textComponent.text = string.Empty;
                StartDialogue();
            }
            else
            {
                LoadNext();
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
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(1f);

        NextLine();
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

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
