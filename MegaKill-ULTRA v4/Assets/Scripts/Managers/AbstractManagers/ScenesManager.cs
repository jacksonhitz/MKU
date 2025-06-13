using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour, IInteractable
{
    //SEPERATE SCENE MANAGEMENT FROM STATEMANAGER
    //HANDLE SAME SCENE STATES (PAUSE, FILE, TRANSITION)
    //SPECIFIC SCENES HANDLES WINCON

    public static ScenesManager Instance { get; set; }

    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    protected GameObject lvl;

    public virtual void Interact() { }
    protected virtual void Awake()
    {
        Instance = this;
        lvl = transform.GetChild(0).gameObject;
    }
    protected virtual void Update()
    {
        if (StateManager.IsPassive()) lvl?.SetActive(false);
        else lvl?.SetActive(true);
    }

    public void ElimCheck()
    {
        if (lvlType == WinCon.Elim) StateManager.LoadNext(); //LoadScore();
    }

    public void LoadScore()
    {
        StartCoroutine(StateManager.LoadState(StateManager.GameState.SCORE, 2f));
    }

}
