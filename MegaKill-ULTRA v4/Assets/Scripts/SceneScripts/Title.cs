public class Title : SceneScript
{
    private new void Awake()
    {
        base.Awake();
        LevelActive = true;
    }

    private new void Start()
    {
        base.Start();
        StartLevel();
    }

    public override void StartLevel()
    {
        SoundManager.Instance.Play("Title");
        State = StateManager.SceneState.FILE;
    }

    public void StartGame()
    {
        _ = StateManager.LoadLevel(StateManager.GameState.TUTORIAL, 1f, destroyCancellationToken);
    }
}
