public class Title : SceneScript
{
    private new void Awake()
    {
        base.Awake();
        LevelActive = true;
    }

    public override void StartLevel()
    {
        SoundManager.Instance.CreateSoundBuilder().Play("Title");
        State = StateManager.SceneState.FILE;
    }

    public void StartGame()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.TUTORIAL, 1f, destroyCancellationToken);
    }
}
