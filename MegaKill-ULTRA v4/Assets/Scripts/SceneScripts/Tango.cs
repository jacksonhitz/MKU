using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tango : ScenesManager
{
    Dialogue dialogue;
    PopUp popUp;
    SoundManager sound;

    int dosedCount;
    bool started;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;

        dialogue = FindObjectOfType<Dialogue>();
        popUp = FindObjectOfType<PopUp>();

        if (SceneManager.GetActiveScene().name != "TANGO")
            StartCoroutine(StateManager.LoadState(StateManager.GameState.TANGO, 0f));
        else
            StateManager.LoadSilent(StateManager.GameState.TANGO);
    }

    protected override void Start()
    {
        base.Start();

        sound = SoundManager.Instance;
        sound.Play("Witch");

        dialogue.TypeText("F TO HAND OUT MKU");
    }

    //DOSED WITH MKU
    public override void Interact()
    {
        if (dosedCount == 0)
            dialogue.Off();

        popUp.UpdatePopUp("MKU DISTRIBUTED");
        dosedCount++;
        Debug.Log("dosed");

        if (dosedCount > 10 && !started)
            StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        started = true;

        dialogue.TypeText("LADIES AND GENTLEMEN! THE GROOVES WILL START IN 1 MINUTE, MAKE YOUR WAY TO THE MAIN STAGE!");
        yield return new WaitForSeconds(10f);
        dialogue.Off();
        yield return new WaitForSeconds(20f);
        dialogue.TypeText("30 SECONDS!");
        yield return new WaitForSeconds(10f);
        dialogue.Off();
        yield return new WaitForSeconds(10f);
        dialogue.TypeText("10!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("9!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("8!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("7!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("6!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("5!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("4!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("3!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("2!");
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("1!");
        yield return new WaitForSeconds(1f);
        dialogue.Off();

        StartCoroutine(StateManager.LoadState(StateManager.GameState.TANGO2, 0f));
        sound.Play("Acid");
        InteractionManager.Instance.ExtractOn();
        EnemyManager.Instance.Brawl();

        dialogue.TypeText("F ON ANY VAN TO EXTRACT");
    }
}
