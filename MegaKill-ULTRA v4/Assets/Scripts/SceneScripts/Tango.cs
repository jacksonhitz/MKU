using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tango : SceneScript
{
    [SerializeField]
    Dialogue dialogue;

    [SerializeField]
    PopUp popUp;

    int dosedCount;
    bool started;

    public override void StartLevel()
    {
        _ = dialogue.TypeText("F TO GIVE DRUGS");
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
    }

    void NewsDialogue()
    {
        dialogue.TypeText(
            "We are just now receiving reports from the authorities that an underground USSR base has been discovered"
                + " operating out of the abandoned downtown subway system - that's right folks, Reds here on American soil...  "
        );
    }
}
