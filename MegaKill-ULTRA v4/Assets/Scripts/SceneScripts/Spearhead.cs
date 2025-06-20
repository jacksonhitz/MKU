using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spearhead : SceneScript
{
    public override void StartLevel()
    {
        base.StartLevel();
        SoundManager.Instance.Play("DJ");
    }
}
