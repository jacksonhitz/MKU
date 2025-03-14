using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateManager 
{
    public enum GameState
    {
        Title,
        Intro,
        Tutorial,
        Lvl,
        Paused,
        Score
    }
    public static GameState state = GameState.Title;
}
