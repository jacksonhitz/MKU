using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    DialogueData[] lines;

    void Awake()
    {
        Instance = this;

        lines = Resources.LoadAll<DialogueData>("Dialogue");
        Debug.Log("Loaded Dialogues: " + lines.Length);
    }

    public DialogueData GetRandomLine(int trip)
    {
        var matching = System.Array.FindAll(lines, d => d.trip == trip);
        return matching[Random.Range(0, matching.Length)];
    }
}
