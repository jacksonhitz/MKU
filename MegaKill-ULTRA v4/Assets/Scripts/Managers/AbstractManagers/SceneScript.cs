using System;
using System.Threading.Tasks;
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
            _state = value;
            StateChanged?.Invoke(_state);
        }
    }

    private GameObject level;

    protected bool LevelActive
    {
        get => level.activeInHierarchy;
        set => level.SetActive(value);
    }

    protected virtual void Awake()
    {
        Instance = this;
        if (StateManager.Level == StateManager.GameState.TITLE)
            StateManager.DebugSetLevel(StateManager.LevelMapping[GetType()]);
        level = transform.GetChild(0).gameObject;
        StateChanged?.Invoke(SceneState.TRANSITION);
        if (StateManager.PreviousLevel != StateManager.Level)
        {
            level.SetActive(false);
        }

        Interactable.InteractableUsed += OnInteract;
        EnemyManager.EnemyKilled += OnEnemyKilled;
    }

    protected virtual void Start()
    {
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
        level.SetActive(true);
        StateChanged?.Invoke(SceneState.PLAYING);
    }

    public virtual void ExitLevel()
    {
        State = SceneState.TRANSITION;
        PlayerController.Instance?.gameObject.transform.SetParent(null);
    }

    public virtual void EndLevel()
    {
        level.SetActive(false);
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
        NewsDialogue().Forget();
    }

    [ConsoleMethod("EndLevel", "Ends the current level and goes to the score screen")]
    public static void EndLevelCommand()
    {
        Instance.EndLevel();
    }

    protected virtual async UniTaskVoid NewsDialogue()
    {
        await Dialogue.Instance.TypeText("PRESS SPACE TO CONTINUE").WaitForComplete();
    }
}
