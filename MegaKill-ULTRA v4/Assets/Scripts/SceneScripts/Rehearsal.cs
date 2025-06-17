using System;
using Cysharp.Threading.Tasks;

public class Rehearsal : SceneScript
{
    protected override async UniTaskVoid NewsDialogue()
    {
        await Dialogue
            .Instance.TypeText(
                "We are just now receiving reports from the authorities that an underground USSR base has been discovered"
                    + " operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...  "
            )
            .WaitForComplete();
        base.NewsDialogue().Forget();
    }

    public override void StartLevel()
    {
        base.StartLevel();
        SoundManager.Instance.Play("Acid");
    }

    protected override void OnEnemyKilled((Type type, int enemiesRemaining) tuple)
    {
        if (tuple.enemiesRemaining != 0 || State != StateManager.SceneState.PLAYING)
            return;

        EndLevel();
    }
}
