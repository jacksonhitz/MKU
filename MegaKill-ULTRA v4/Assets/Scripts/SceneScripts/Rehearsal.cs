using System;
using AudioSystem;

public class Rehearsal : SceneScript
{
    public override void StartLevel()
    {
        newsDialogue = new()
        {
            "We are just now receiving reports from the authorities that an underground USSR base has been discovered"
                + " operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...",
        };
        base.StartLevel();
        MusicManager.Instance.Play(musicTracks[0]);
        PlayerController.Instance.commandUI.Active = true;
    }

    protected override void OnEnemyKilled((Type type, int enemiesRemaining) tuple)
    {
        if (tuple.enemiesRemaining != 0 || State != StateManager.SceneState.PLAYING)
            return;

        EndLevel();
    }
}
