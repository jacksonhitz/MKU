using System.Collections;
using AudioSystem;
using IngameDebugConsole;
using UnityEngine;

public class Tango : SceneScript
{
    private int dosedCount;
    private bool started;
    private bool extractsActive;

    public override void StartLevel()
    {
        newsDialogue = new()
        {
            "We are just now receiving reports from the authorities that an underground USSR base has been discovered"
                + " operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...  ",
        };
        base.StartLevel();
        EnemyManager.Instance.EnemySpawning = false;
        MusicManager.Instance.Play(musicTracks[0]);
        Dialogue.Instance.TypeText("F TO GIVE DRUGS");
        DebugLogConsole.AddCommandInstance(
            "SkipToPhase2",
            "Skip to phase 2 of Tango",
            nameof(Phase2),
            this
        );

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
            Dialogue.Instance.Off();

        DialogueManager.Instance.PlayRandomLine();

        PlayerController.Instance.popUpUI.UpdatePopUp("MKU DISTRIBUTED");
        dosedCount++;

        if (dosedCount > 10 && !started)
            StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        started = true;

        PlayerController.Instance.dialogueUI.TypeText(
            "LADIES AND GENTLEMEN! THE GROOVES WILL START IN 1 MINUTE, MAKE YOUR WAY TO THE MAIN STAGE!"
        );
        yield return new WaitForSeconds(10f);
        PlayerController.Instance.dialogueUI.Off();
        yield return new WaitForSeconds(20f);
        PlayerController.Instance.dialogueUI.TypeText("30 SECONDS!");
        yield return new WaitForSeconds(10f);
        PlayerController.Instance.dialogueUI.Off();
        yield return new WaitForSeconds(10f);
        for (int i = 10; i > 0; i--)
        {
            PlayerController.Instance.dialogueUI.TypeText($"{i}!");
            yield return new WaitForSeconds(1f);
        }
        PlayerController.Instance.dialogueUI.Off();
        PlayerController.Instance.commandUI.Active = true;
        yield return new WaitForSeconds(2.5f);
        Phase2();
    }

    public void Phase2()
    {
        EnemyManager.Instance.EnemySpawning = true;
        MusicManager.Instance.Play(musicTracks[1]);
        InteractionManager.Instance.ExtractOn();
        EnemyManager.Instance.Brawl();
        extractsActive = true;
        PlayerController.Instance.dialogueUI.TypeText("F ON ANY VAN TO EXTRACT");
    }

    private IEnumerator DanceTimer(Enemy enemy)
    {
        int delay = Random.Range(0, 10);
        yield return new WaitForSeconds(delay);
        enemy.isDance = true;
    }
}
