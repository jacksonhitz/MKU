using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sable : SceneScript
{
    public override void StartLevel()
    {
        base.StartLevel();
        SoundManager.Instance.CreateSoundBuilder().Play("4L");
    }
}
