using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleUI : MonoBehaviour
{
    [SerializeField] Dialogue introText;
    [SerializeField] Dialogue tangoText;
    [SerializeField] GameObject titleScreen;

    [SerializeField] GameObject settingsScreen;

    public void StartButton()
    {
        introText.Play();
        titleScreen.SetActive(false);
    }

    //Not sure if this will be used as it's simple to connect to them through the Unity UI
    public void ChaptersButton()
    {

    }

    public void InstructionButton()
    {
        StartCoroutine(StateManager.LoadState(StateManager.GameState.TUTORIAL, 2f));
    }
    public void RehearsalButton()
    {
        StartCoroutine(StateManager.LoadState(StateManager.GameState.REHEARSAL, 2f));
    }
    public void TangoButton()
    {
        StartCoroutine(StateManager.LoadState(StateManager.GameState.TANGO, 2f));
    }
    public void SableButton()
    {
        StartCoroutine(StateManager.LoadState(StateManager.GameState.SABLE, 2f));
    }
    public void SpearheadButton()
    {
        StartCoroutine(StateManager.LoadState(StateManager.GameState.SPEARHEAD, 2f));
    }
    public void OptionsButton()
    {
        settingsScreen.SetActive(true);
    }
    public void ExitButton()
    {
        Debug.Log("Quitting/Exiting Application");
        Application.Quit();
    }

    public void TangoFile()
    {
        tangoText.Play();
        titleScreen.SetActive(false);
    }
}
