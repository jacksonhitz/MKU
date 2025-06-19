using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tango : SceneScript
{
    [SerializeField]
    Dialogue dialogue;

    [SerializeField]
    PopUp popUp;

    private int dosedCount;
    private bool started;
    private bool extractsActive;

    public override void StartLevel()
    {
        base.StartLevel();
        SoundManager.Instance.Play("Witch");
        _ = dialogue.TypeText("F TO GIVE DRUGS");

        foreach (Enemy enemy in EnemyManager.Instance.enemies)
        {
            if (enemy.currentState is not Enemy.EnemyState.Static)
                continue;
            int rand = Random.Range(0, 3);
            if (rand == 0)
                StartCoroutine(DanceTimer(enemy));
        }
    }

    //DOSED WITH MKU
    protected override void OnInteract((Interactable.Type type, Interactable interactable) tuple)
    {
        if (tuple.type is Interactable.Type.Extract && extractsActive)
        {
            EndLevel();
            // TODO: Add van sound
            return;
        }

        if (tuple.type is not Interactable.Type.Enemy)
            return;

        if (dosedCount == 0)
            dialogue.Off();

        DialogueManager.Instance.PlayRandomLine();

        popUp.UpdatePopUp("MKU DISTRIBUTED");
        dosedCount++;

        if (dosedCount > 10 && !started)
            StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        started = true;

        dialogue.TypeText(
            "LADIES AND GENTLEMEN! THE GROOVES WILL START IN 1 MINUTE, MAKE YOUR WAY TO THE MAIN STAGE!"
        );
        yield return new WaitForSeconds(10f);
        dialogue.Off();
        yield return new WaitForSeconds(20f);
        dialogue.TypeText("30 SECONDS!");
        yield return new WaitForSeconds(10f);
        dialogue.Off();
        yield return new WaitForSeconds(10f);
        for (int i = 10; i > 0; i--)
        {
            dialogue.TypeText($"{i}!");
            yield return new WaitForSeconds(1f);
        }
        dialogue.Off();

        SoundManager.Instance.Play("Magic");
        InteractionManager.Instance.ExtractOn();
        EnemyManager.Instance.Brawl();
        dialogue.TypeText("F ON ANY VAN TO EXTRACT");
        extractsActive = true;
    }

    protected override async UniTaskVoid NewsDialogue()
    {
        await dialogue
            .TypeText(
                "We are just now receiving reports from the authorities that an underground USSR base has been discovered"
                    + " operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...  "
            )
            .WaitForComplete();
        base.NewsDialogue().Forget();
    }

    private IEnumerator DanceTimer(Enemy enemy)
    {
        int delay = Random.Range(0, 10);
        yield return new WaitForSeconds(delay);
        enemy.isDance = true;
    }
}
