using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using SceneState = StateManager.SceneState;

public abstract class SceneScript : MonoBehaviour
{
    [ResetOnPlay]
    public static SceneScript Instance { get; private set; }

    [ResetOnPlay]
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
    public GameObject LevelRoot => level;

    protected List<string> newsDialogue = new();

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
        PlayerHealth.PlayerDied += OnPlayerDied;
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
        InputManager.UIActionMap.Submit.performed += SubmitOnPerformed;

        void SubmitOnPerformed(InputAction.CallbackContext ctx)
        {
            InputManager.UIActionMap.Submit.performed -= SubmitOnPerformed;
            StartLevel();
        }
    }

    protected void OnDestroy()
    {
        Interactable.InteractableUsed -= OnInteract;
        EnemyManager.EnemyKilled -= OnEnemyKilled;
        PlayerHealth.PlayerDied -= OnPlayerDied;
    }

    protected virtual void OnPlayerDied()
    {
        PlayerController.Instance.Active = false;
        StateManager.RestartLevel(2f, Application.exitCancellationToken).Forget();
        State = SceneState.TRANSITION;
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

    public void Transition()
    {
        State = SceneState.TRANSITION;
    }

    private void ExitLevel()
    {
        StateManager.LoadNext();
        State = SceneState.TRANSITION;
    }

    protected void EndLevel()
    {
        level.SetActive(false);
        PlayerController.Instance.Active = false;
        GoToScoreScreen();
    }

    private void GoToScoreScreen()
    {
        if (State == SceneState.SCORE)
            return;
        State = SceneState.SCORE;
        SoundManager.Instance.StopAll();
        SoundManager.Instance.MusicOff();
        ScoreUI.Instance.Visible = true;
        SoundManager.Instance.CreateSoundBuilder().Play("All");
        NewsDialogue();
    }

    private void NewsDialogue()
    {
        // TODO: Show correct input name based on binding
        newsDialogue.Add("PRESS SPACE TO CONTINUE");
        Dialogue.Instance.lines = newsDialogue.ToArray();
        var textTask = Dialogue.Instance.Play();
        InputManager.UIActionMap.Submit.performed += SubmitOnPerformed;

        void SubmitOnPerformed(InputAction.CallbackContext obj)
        {
            if (textTask.Status is UniTaskStatus.Pending)
            {
                PlayerController.Instance.dialogueUI.Complete();
            }
            else
            {
                InputManager.UIActionMap.Submit.performed -= SubmitOnPerformed;
                ExitLevel();
            }
        }
    }

    [ConsoleMethod("EndLevel", "Ends the current level and goes to the score screen")]
    public static void EndLevelCommand()
    {
        Instance.EndLevel();
    }
}
