using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spearhead : SceneScript
{
    void Start()
    {
        SoundManager.Instance.Play("DJ");
    }

    public override void StartLevel() { }
}
