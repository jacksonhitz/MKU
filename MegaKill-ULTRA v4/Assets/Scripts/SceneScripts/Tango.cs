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

        dialogue.TypeText("F TO HAND OUT MKU", 0f);
    }

    //DOSED WITH MKU
    public override void Interact()
    {
        if (dosedCount == 0)
            dialogue.Off();

        DialogueManager.Instance.PlayRandomLine();

        popUp.UpdatePopUp("MKU DISTRIBUTED");
        dosedCount++;
        Debug.Log("dosed");

        if (dosedCount > 10 && !started)
            StartCoroutine(Countdown());
    }

    //YO WHAT THE HELL WAS I THINKING WHEN I MADE THIS BULLSHIT
    IEnumerator Countdown()
    {
        started = true;

        dialogue.TypeText("LADIES AND GENTLEMEN! THE GROOVES WILL START IN 1 MINUTE, MAKE YOUR WAY TO THE MAIN STAGE!", 0f);
        yield return new WaitForSeconds(10f);
        dialogue.Off();
        yield return new WaitForSeconds(20f);
        dialogue.TypeText("30 SECONDS!", 0f);
        yield return new WaitForSeconds(10f);
        dialogue.Off();
        yield return new WaitForSeconds(10f);
        dialogue.TypeText("10!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("9!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("8!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("7!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("6!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("5!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("4!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("3!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("2!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.TypeText("1!", 0f);
        yield return new WaitForSeconds(1f);
        dialogue.Off();

        StartCoroutine(StateManager.LoadState(StateManager.GameState.TANGO2, 0f));
        sound.Play("Acid");
        InteractionManager.Instance.ExtractOn();
        EnemyManager.Instance.Brawl();

        dialogue.TypeText("F ON ANY VAN TO EXTRACT", 0f);
    }
}
