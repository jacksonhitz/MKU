using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Russia : SceneScript
{
    public override void StartLevel()
    {
        base.StartLevel();
        SoundManager.Instance.Play("4L");
    }
}
