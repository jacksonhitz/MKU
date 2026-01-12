using System.Collections.Generic;
using UnityEngine;
using UnityUtils;
using Debug = UnityEngine.Debug;

public class DialogueManager : Singleton<DialogueManager>
{
    [ResetOnPlay]
    DialogueData[] lines;

    float cooldown = .5f;
    float timer;

    protected override void Awake()
    {
        base.Awake();
        lines = Resources.LoadAll<DialogueData>("Dialogue");
        Debug.Log($"Loaded Dialogues: {lines.Length}");
    }

    public void PlayRandomLine()
    {
        if (Time.time < timer)
            return;

        timer = Time.time + cooldown;

        Debug.Log("Called");

        var currentState = StateManager.Level;
        List<DialogueData> matching = new();

        foreach (var dialogue in lines)
        {
            if (dialogue.gameState == currentState)
                matching.Add(dialogue);
        }

        if (matching.Count == 0)
        {
            Debug.LogWarning($"No dialogue for state {currentState}");
            return;
        }

        var chosen = matching[Random.Range(0, matching.Count)];
        Debug.Log(chosen.line);
        Dialogue.Instance.TypeText(chosen.line);
    }
}
