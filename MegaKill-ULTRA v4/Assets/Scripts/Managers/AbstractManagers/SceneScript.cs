using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using SceneState = StateManager.SceneState;

public abstract class SceneScript : MonoBehaviour, IInteractable
{
    //SEPERATE SCENE MANAGEMENT FROM STATEMANAGER
    //HANDLE SAME SCENE STATES (PAUSE, FILE, TRANSITION)
    //SPECIFIC SCENES HANDLES WINCON

    public static SceneScript Instance { get; set; }
    public static event Action<SceneState> StateChanged;

    [field: ReadOnly]
    [field: SerializeField]
    public SceneState State { get; protected set; } = SceneState.TRANSITION;

    // TODO: Remove this shit
    protected GameObject lvl;

    public virtual void Interact() { }

    protected virtual void Awake()
    {
        Instance = this;
        lvl = transform.GetChild(0).gameObject;
        lvl.SetActive(false);
    }

    protected virtual void Start()
    {
        if (FileUI.Instance)
        {
            FileUI.Instance.Visible = true;
            State = SceneState.FILE;
        }
        else
            Debug.LogWarning("FileUI is NULL");
    }

    public virtual void StartLevel()
    {
        FileUI.Instance.Visible = false;
        State = SceneState.PLAYING;
        lvl.SetActive(true);
    }

    public virtual void EndLevel()
    {
        State = SceneState.TRANSITION;
        // lvl.SetActive(false);
    }

    public void ElimCheck()
    {
        // TODO: FIX ELIM CHECK LEVEL COND
        // if (lvlType == WinCon.Elim) StateManager.LoadNext(); //LoadScore();
    }

    public async UniTask LoadScore()
    {
        if (State == SceneState.SCORE)
            return;
        State = SceneState.SCORE;
        // TODO: FIX SCORE SCENE SHOWING
        // StartCoroutine(StateManager.LoadState(StateManager.GameState.SCORE, 2f));
        StateChanged?.Invoke(SceneState.SCORE);
        await SceneManager.LoadSceneAsync("OUTRO", LoadSceneMode.Additive);
        ScoreUI.Instance.Visible = true;
    }
}
