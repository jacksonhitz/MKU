using System.Collections;
using UnityEngine;

public class Festival : MonoBehaviour
{
    EnemyManager enemyManager;
    SoundManager soundManager;
    Dialogue dialogue;
    PopUp popUp;
    ItemManager itemManager;

    int dosedCount;

    bool started;

    void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        itemManager = FindObjectOfType<ItemManager>();
        dialogue = FindObjectOfType<Dialogue>();
        popUp = FindObjectOfType<PopUp>();
    }

    void Start()
    {
        StateManager.LoadState(StateManager.GameState.TANGO);
        itemManager?.CollectItems();
        dialogue.TypeText("Q / E TO HAND OUT MKU");
    }

    public void Dosed(Enemy enemy)
    {
        if (dosedCount == 0)
        {
            dialogue.Off();
        }

        if (!enemy.dosed)
        {
            popUp.UpdatePopUp("DRUGS DISTRIBUTED");
            soundManager.GiveDrug();

            enemy.dosed = true;
            enemy.friendly = false;
            dosedCount++;
            Debug.Log("dosed");
            if (dosedCount > 10 && !started)
            {
                StartCoroutine(Countdown());
            }
        }
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

        StateManager.LoadState(StateManager.GameState.FIGHT);
        enemyManager.Brawl();
    }
}
