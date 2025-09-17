using AudioSystem;

public class Sable : SceneScript
{
    public override void StartLevel()
    {
        base.StartLevel();
        MusicManager.Instance.Play(musicTracks[0]);
    }
}
