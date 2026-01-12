using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Line", fileName = "Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(3, 10)]
    public string line;
    public StateManager.GameState gameState;
    public int trip;
}
