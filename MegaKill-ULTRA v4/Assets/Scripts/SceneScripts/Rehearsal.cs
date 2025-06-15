using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rehearsal : SceneScript
{
    void NewsDialogue()
    {
        _ = Dialogue.Instance.TypeText(
            "We are just now receiving reports from the authorities that an underground USSR base has been discovered"
                + " operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...  "
        );
        _ = Dialogue.Instance.TypeText("PRESS SPACE TO CONTINUE");
    }

    public override void StartLevel()
    {
        base.StartLevel();
        SoundManager.Instance.Play("Acid");
    }
}
