using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tango : ScenesManager
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] PopUp popUp;

    int dosedCount;
    bool started;

    protected override void Awake()
    {
        base.Awake();
        StateManager.lvl = StateManager.GameState.TANGO;
        if (StateManager.State != StateManager.GameState.FILE)
            StateManager.StartLvl();
    }

    void OnEnable()
    {
        StateManager.OnStateChanged += StateChange;
    }
    void OnDisable()
    {
        StateManager.OnStateChanged -= StateChange;
    }
    void StateChange(StateManager.GameState state)
    {
        switch (state)
        {
            case StateManager.GameState.TANGO: StartLvl(); break;
        }
    }

    void StartLvl()
    {
        dialogue.TypeText("F TO GIVE DRUGS", 0f);
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
        SoundManager.Instance.Play("Acid");
        InteractionManager.Instance.ExtractOn();
        EnemyManager.Instance.Brawl();

        dialogue.TypeText("F ON ANY VAN TO EXTRACT", 0f);
    }
}
