using AudioSystem;

public class Spearhead : SceneScript
{
    public override void StartLevel()
    {
        base.StartLevel();
        MusicManager.Instance.Play(musicTracks[0]);
    }
}
