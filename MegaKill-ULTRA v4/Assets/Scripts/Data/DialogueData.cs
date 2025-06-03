using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue")]
public class DialogueData : ScriptableObject
{
    //MERGE W SOUND DATA DIALOGUE SOON
    [TextArea(3, 10)]
    public string[] lines;
    public int trip;
}

