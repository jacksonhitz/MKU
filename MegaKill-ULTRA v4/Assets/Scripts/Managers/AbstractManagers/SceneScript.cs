using System;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using SceneState = StateManager.SceneState;

public abstract class SceneScript : MonoBehaviour
{
    //HANDLE SAME SCENE STATES (PAUSE, FILE, TRANSITION)

    public static SceneScript Instance { get; set; }
    public static event Action<SceneState> StateChanged;

    [ReadOnly]
    [SerializeField]
    private SceneState _state = SceneState.TRANSITION;

    public SceneState State
    {
        get => _state;
        protected set
        {
            if (value == _state)
                return;
            StateChanged?.Invoke(value);
            _state = value;
        }
    }

    // TODO: Remove this shit
    protected GameObject lvl;

    protected virtual void Awake()
    {
        Instance = this;
        lvl = transform.GetChild(0).gameObject;
        StateChanged?.Invoke(SceneState.TRANSITION);
        if (StateManager.PreviousLevel != StateManager.Level)
        {
            lvl.SetActive(false);
        }

        Interactable.InteractableUsed += OnInteract;
        EnemyManager.EnemyKilled += OnEnemyKilled;
    }

    protected virtual void Start()
    {
        // TODO: Only show file UI on first load, not on respawns
        if (!FileUI.Instance)
        {
            Debug.LogWarning("FileUI is NULL");
            StartLevel();
            return;
        }

        if (!StateManager.IsFirstAttempt)
        {
            StartLevel();
            return;
        }

        FileUI.Instance.Visible = true;
        State = SceneState.FILE;
    }

    protected void OnDestroy()
    {
        Interactable.InteractableUsed -= OnInteract;
        EnemyManager.EnemyKilled -= OnEnemyKilled;
    }

    protected virtual void OnInteract((Interactable.Type type, Interactable interactable) tuple) { }

    protected virtual void OnEnemyKilled((Type type, int enemiesRemaining) tuple) { }

    public virtual void StartLevel()
    {
        FileUI.Instance.Visible = false;
        State = SceneState.PLAYING;
        lvl.SetActive(true);
        StateChanged?.Invoke(SceneState.PLAYING);
    }

    public virtual void ExitLevel()
    {
        State = SceneState.TRANSITION;
        PlayerController.Instance?.gameObject.transform.SetParent(null);
    }

    public virtual void EndLevel()
    {
        lvl.SetActive(false);
        GoToScoreScreen();
    }

    private void GoToScoreScreen()
    {
        if (State == SceneState.SCORE)
            return;
        State = SceneState.SCORE;
        SoundManager.Instance.Stop();
        SoundManager.Instance.MusicOff();
        ScoreUI.Instance.Visible = true;
        SoundManager.Instance.Play("All");
    }

    [ConsoleMethod("EndLevel", "Ends the current level and goes to the score screen")]
    public static void EndLevelCommand()
    {
        Instance.EndLevel();
    }
}
