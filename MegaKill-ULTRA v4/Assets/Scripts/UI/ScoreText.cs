using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    string[] lines;
    public float textSpeed;
    ScoreManager scoreManager;

    int index;

    void Awake()
    {
        scoreManager = FindObjectOfType<ScoreManager>();

    }

    void Start()
    {
        lines[0] = "TIME: " + scoreManager.timeCount;
        lines[1] = "KILLS: " + scoreManager.killCount;
        lines[2] = "STYLE: " + scoreManager.styleCount;
        lines[3] = "BRUTALITY: " + scoreManager.brutalityCount;
        lines[4] = "PRECISION: " + scoreManager.prescionCount;

        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        yield return new WaitForSeconds(1f);
        if (index > 0)
            textComponent.text += "\n";

        yield return new WaitForSeconds(.1f);
        //soundManager.NewLine();

        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        //soundManager.StopLine();
        NextLine();
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
    }
}
