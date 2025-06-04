using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class ScenesManager : MonoBehaviour, IInteractable
{
    public static ScenesManager Instance { get; set; }

    public enum WinCon { Elim, Extract, Target }
    public WinCon lvlType;

    public virtual void Interact() { }
}
