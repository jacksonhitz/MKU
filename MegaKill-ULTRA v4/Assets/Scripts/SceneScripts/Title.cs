using AudioSystem;

public class Title : SceneScript
{
    private new void Awake()
    {
        base.Awake();
        LevelActive = true;
    }

    public override void StartLevel()
    {
        MusicManager.Instance.Play(musicTracks[0]);
        State = StateManager.SceneState.FILE;
    }

    public void StartGame()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.TUTORIAL, 1f, destroyCancellationToken);
    }
}
